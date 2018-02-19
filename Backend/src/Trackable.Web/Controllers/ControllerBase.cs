using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trackable.Common;
using Trackable.Web.Filters;

namespace Trackable.Web
{
    [ModelBindingValidationFilter]
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
