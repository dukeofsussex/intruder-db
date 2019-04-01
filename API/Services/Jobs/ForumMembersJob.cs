// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using API.Models;
    using API.Utils;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ForumMembersJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly AgentJobs agentJobs;

        private readonly int entriesPerPage = 25;

        private readonly IConfiguration config;

        public ForumMembersJob(AgentJobs agentJobs, IConfiguration config)
        {
            this.agentJobs = agentJobs;
            this.config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Grab page to work on from DB
            int page = DB.Get("SELECT * FROM storage WHERE name = 'forum_members.current_page'", null, Create.StorageObject).Value;

            logger.Debug("Fetching member list for page " + page);

            HtmlDocument doc = await Request
                .GetHTMLAsync(new Uri(this.config["Settings:SBGURL"] + "/forum/memberlist.php?search_group_id=8&mode=&sk=c&sd=a&start=" + ((page - 1) * this.entriesPerPage)))
                .ConfigureAwait(false);
            HtmlNode totalPagesNode = doc.DocumentNode.SelectSingleNode("//li[contains(@class, 'pagination')]").Descendants("a").Last();
            HtmlNodeCollection agentRows = doc.DocumentNode.SelectNodes("//table[@id='memberlist']/tbody/tr/td[1]/a");
            HtmlNodeCollection forumGroupAvatars = doc.DocumentNode.SelectNodes("//table[@id='memberlist']/tbody/tr/td[1]/span[contains(@class, 'rank-img')]/img");
            HtmlNodeCollection registrationTimestamps = doc.DocumentNode.SelectNodes("//table[@id='memberlist']/tbody/tr/td[contains(@class, 'joined')]");

            logger.Debug("Fetched member list for page " + page);

            if (!int.TryParse(totalPagesNode.InnerText, out int totalPages))
            {
                logger.Warn("Unable to parse the total pages count from the forum member list");
            }

            // Retrieve data for each agent
            using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(5))
            {
                await Task.WhenAll(agentRows.Select(async (agent, index) =>
                {
                    await concurrencySemaphore.WaitAsync().ConfigureAwait(false);

                    int agentID = await this.agentJobs.GetAgentAsync(
                        0,
                        agent.InnerText,
                        false,
                        new AgentForumProfile()
                        {
                            Role = Util.GetConvertedRole(forumGroupAvatars[index].Attributes["title"].Value),
                            Registered = DateTime.Parse(registrationTimestamps[index].InnerText, CultureInfo.InvariantCulture)
                        })
                        .ConfigureAwait(false);

                    concurrencySemaphore.Release();
                })).ConfigureAwait(false);
            }

            logger.Debug("Finished page " + page + " of " + totalPages);

            // Update page in DB
            if (totalPages != 0 && totalPages > page)
            {
                // Update page in DB
                DB.Update(
                    "UPDATE storage SET value = @page WHERE name = 'forum_members.current_page'",
                    new Dictionary<string, dynamic>()
                    {
                        { "@page", page += 1 }
                    });
            }
        }
    }
}
