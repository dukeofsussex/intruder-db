// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using NLog;

    public static class Request
    {
        private static readonly HttpClient Client = new HttpClient();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string userAgent = "Intruder-DB Bot";

        public static void Setup()
        {
            Client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public static async Task<string> GetAsync(Uri url)
        {
            try
            {
                return await Client.GetStringAsync(url).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                logger.Error(url);
                logger.Error(e);
                return null;
            }
        }

        public static async Task<string> PostAsync(Uri url, Dictionary<string, string> body)
        {
            try
            {
                using (HttpResponseMessage response = await Client.PostAsync(url, new FormUrlEncodedContent(body)).ConfigureAwait(false))
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (HttpRequestException e)
            {
                logger.Error(url);
                logger.Error(body);
                logger.Error(e);
                return null;
            }
        }

        public static async Task<HtmlDocument> GetHTMLAsync(Uri url)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await GetAsync(url).ConfigureAwait(false));
            return doc;
        }
    }
}
