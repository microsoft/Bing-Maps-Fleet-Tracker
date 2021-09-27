// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;

namespace Trackable.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Route("{*url}")]
        public IActionResult Home()
        {
            // Work around for issue with File and If-Modified-Since
            // https://github.com/aspnet/Mvc/issues/6875
            this.HttpContext.Request.Headers.Remove("If-Modified-Since");

            // Do not cache index.html
            this.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            this.HttpContext.Response.Headers.Add("Expires", "-1");

            if (configuration.GetValue<bool>("Serving:ServeFrontend"))
            {
                return File("dist/Index.html", "text/html");
            }

            return NotFound();
        }
    }
}
