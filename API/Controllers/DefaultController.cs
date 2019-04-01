// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System;
    using System.Net;
    using API.Models;
    using Microsoft.AspNetCore.Mvc;
    using NLog;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : Controller
    {
        private static Logger logger = LogManager.GetLogger("CatchAllController");

        [Route("{*url}")]
        public IActionResult Get(Uri url)
        {
            logger.Warn($"Non-existent route requested: {url}");

            return this.NotFound(new ErrorResponse()
            {
                Code = (int)HttpStatusCode.NotFound,
                Message = "API endpoint not found"
            });
        }
    }
}
