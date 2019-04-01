// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using API.Models;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ForumPMJob : IJob
    {
        private const string CookieSIDName = "sbg_session_int_sid";
        private const string Subject = "Intruder-DB agent verification";
        private const string Message = "Hello {0},\nthe following user tried to claim your account on Intruder DB: https://steamcommunity.com/profiles/{1}"
            + "\nTo prevent users from claiming random accounts,"
            + " they are required to validate their account ownership.\nTo claim your account, please follow this link: https://www.intruder-db.info/claim/confirm?id={2}&uid={3}."
            + "\n\nIf this action wasn't performed by you, just ignore this PM and nothing will happen.";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration config;

        public ForumPMJob(IConfiguration config)
        {
            this.config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Claim claim = DB.Get(
                "SELECT claim.uid AS claim_uid, claim.sent AS claim_sent, claim.timestamp AS claim_timestamp,"
                + " id AS profile_id, claim.steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url"
                + " FROM claim INNER JOIN agent ON agent.id = claim.agent_id WHERE sent = 0 ORDER BY timestamp ASC",
                null,
                Create.Claim);

            if (claim != null)
            {
                bool success = await this.SendAsync(claim.Claimer.ID, claim.Claimer.SteamID, claim.Claimer.Name, claim.UID).ConfigureAwait(false);

                if (!success)
                {
                    return;
                }

                DB.Update("UPDATE claim SET sent = 1 WHERE uid = @uid", new Dictionary<string, dynamic>()
                {
                    { "@uid", claim.UID }
                });
            }

            DB.Delete("DELETE FROM claim WHERE timestamp < DATE_SUB(NOW(), INTERVAL 24 HOUR)", null);
        }

        public async Task<bool> SendAsync(int id, long steamID, string username, Guid uid)
        {
            CookieContainer cookieContainer = new CookieContainer();

            Dictionary<string, string> bodyPM = new Dictionary<string, string>()
            {
                { "username_list", string.Empty },
                { "icon", "1" },
                { "subject", Subject },
                { "addbbcode20", "100" },
                { "message", string.Format(CultureInfo.InvariantCulture, Message, username, steamID, id, uid) },
                { "post", "Submit" },
                { "attach_sig", "off" }
            };

            using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Intruder-DB Bot/1.0");

                // Sign in
                try
                {
                    await client.PostAsync(
                            new Uri(this.config["Settings:SBGURL"] + "/forum/ucp.php?mode=login"),
                            new FormUrlEncodedContent(
                                new Dictionary<string, string>()
                                {
                                    { "username", this.config["ForumAccount:Username"] },
                                    { "password", this.config["ForumAccount:Password"] },
                                    { "login", "Login" },
                                }))
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    logger.Error(ex);
                    return false;
                }

                // Retrieve form (need hidden field values)
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(
                        await client.GetStringAsync(
                                new Uri(this.config["Settings:SBGURL"] + "/forum/ucp.php?i=pm&mode=compose"))
                            .ConfigureAwait(false));

                    HtmlNode creationTimeNode = doc.DocumentNode.SelectSingleNode("//input[@name='creation_time']");
                    HtmlNode formTokenNode = doc.DocumentNode.SelectSingleNode("//input[@name='form_token']");
                    HtmlNode statusSwitchNode = doc.DocumentNode.SelectSingleNode("//input[@name='status_switch']");

                    bodyPM.Add("lastclick", creationTimeNode.Attributes["value"].Value);
                    bodyPM.Add("status_switch", statusSwitchNode.Attributes["value"].Value);
                    bodyPM.Add("creation_time", creationTimeNode.Attributes["value"].Value);
                    bodyPM.Add("form_token", formTokenNode.Attributes["value"].Value);
                }
                catch (HttpRequestException ex)
                {
                    logger.Error(ex);
                    return false;
                }

                Stopwatch watch = new Stopwatch();
                watch.Start();

                // Retrieve user id
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(
                        await client.GetStringAsync(
                                new Uri(this.config["Settings:SBGURL"] + $"/forum/memberlist.php?username={username}&mode=searchuser"))
                            .ConfigureAwait(false));

                    HtmlNode agent = doc.DocumentNode.SelectSingleNode("//table[@id='memberlist']/tbody/tr/td[1]/a");
                    if (agent == null)
                    {
                        return false;
                    }

                    string anchor = agent.Attributes["href"].Value;
                    NameValueCollection parameters = HttpUtility.ParseQueryString(anchor.Substring(anchor.IndexOf('?')));
                    if (parameters["u"] != null)
                    {
                        bodyPM.Add($"address_list[u][{parameters["u"]}]", "to"); // Only works on Ubuntu
                    }
                    else
                    {
                        bodyPM.Add($"address_list[u][{parameters["amp;u"]}]", "to"); // Only works on Windows 7
                    }
                }
                catch (HttpRequestException ex)
                {
                    logger.Error(ex);
                    return false;
                }

                watch.Stop();

                if (watch.ElapsedMilliseconds < 2000)
                {
                    await Task.Delay(2000 - (int)watch.ElapsedMilliseconds).ConfigureAwait(false);
                }

                try
                {
                    // Send PM
                    await client.PostAsync(
                            new Uri(
                                this.config["Settings:SBGURL"] + "/forum/ucp.php?i=pm&mode=compose&action=post&sid="
                                    + cookieContainer.GetCookies(new Uri(this.config["Settings:SBGURL"]))[CookieSIDName].Value),
                            new FormUrlEncodedContent(bodyPM))
                        .ConfigureAwait(false);

                    // Sign out
                    await client.GetAsync(
                            new Uri(
                                this.config["Settings:SBGURL"] + "/forum/ucp.php?mode=logout&sid="
                                    + cookieContainer.GetCookies(new Uri(this.config["Settings:SBGURL"]))[CookieSIDName].Value))
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    logger.Error(ex);
                    return false;
                }
            }

            return true;
        }
    }
}
