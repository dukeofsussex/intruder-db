// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using API.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("sbg")]
    public class SBGController : Controller
    {
        private readonly IConfiguration config;

        public SBGController(IConfiguration config)
        {
            this.config = config;
        }

        // GET: sbg/news
        [HttpGet("news")]
        public async Task<IActionResult> GetNewsAsync()
        {
            string feedAddress = await Services.Request.GetAsync(new Uri(this.config["Settings:SBGURL"] + "/intruderFeedAddress.txt")).ConfigureAwait(false);

            string newsResponse = await Services.Request.GetAsync(new Uri(feedAddress)).ConfigureAwait(false);

            if (newsResponse.Length > 0)
            {
                return this.Ok(JObject.Parse(newsResponse));
            }

            return this.NotFound(new ErrorResponse()
            {
                Code = (int)HttpStatusCode.NotFound,
                Message = "News not found"
            });
        }

        // GET: news/official
        [HttpGet("news/official")]
        public async Task<IActionResult> GetOfficialNewsAsync()
        {
            string newsResponse = await Services.Request.GetAsync(new Uri(this.config["Settings:SBGURL"] + "/intruderHQNews.txt")).ConfigureAwait(false);

            if (newsResponse.Length > 0)
            {
                return this.Ok(newsResponse);
            }

            return this.NotFound(new ErrorResponse()
            {
                Code = (int)HttpStatusCode.NotFound,
                Message = "News not found"
            });
        }
    }
}
