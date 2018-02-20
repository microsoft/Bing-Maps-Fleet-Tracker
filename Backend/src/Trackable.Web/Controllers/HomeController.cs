using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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

            if (configuration.GetValue<bool>("Serving:ServeFrontend"))
            {
                return File("dist/Index.html", "text/html");
            }

            return NotFound();
        }
    }
}
