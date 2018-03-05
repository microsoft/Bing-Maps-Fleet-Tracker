// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;

namespace Trackable.Web.Controllers
{
    [Route("api/ping")]
    public class PingController : Controller
    {
        // GET api/ping
        [HttpGet("")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
