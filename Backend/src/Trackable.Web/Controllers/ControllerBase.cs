using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trackable.Common;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Trackable.Web
{
    [Authorize(UserRoles.Viewer, AuthenticationSchemes = "Bearer, OpenIdConnect")]
    public class ControllerBase : Controller
    {
        protected ILoggerFactory LoggerFactory { get; }

        public ControllerBase(ILoggerFactory loggerFactory)
        {
            this.LoggerFactory = loggerFactory;
        }
    }
}
