// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocsController : Controller
    {
        [Route("")]
        public RedirectResult Get()
        {
            return new RedirectResult("~/docs");
        }
    }
}
