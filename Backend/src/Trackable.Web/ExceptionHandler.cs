using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Common.Exceptions;
using Trackable.Services;
using Trackable.TripDetection.Exceptions;

namespace Trackable.Web
{
    public class ExceptionHandler : ExceptionFilterAttribute
    {
        private readonly ILogger logger;

        private readonly IInstrumentationService instrumentationService;

        public ExceptionHandler(
            ILoggerFactory loggerFactory,
            IInstrumentationService instrumentationService)
        {
            this.logger = loggerFactory.CreateLogger<ExceptionHandler>();
            this.instrumentationService = instrumentationService;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            var result = new ContentResult();

            if (exception is ExceptionBase)
            {
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.Content = exception.Message;
                result.ContentType = "text/plain";
                logger.LogWarning(exception);
                await instrumentationService.PostWarningAsync(exception.ToString());
            }
            else if (exception is ModuleConfigurationException)
            {
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                result.Content = exception.Message;
                result.ContentType = "text/plain";
                logger.LogWarning(exception);
            }
            else
            {
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                result.Content = "An error occured while processing your request";
                result.ContentType = "text/plain";
                logger.LogError(exception);
                await instrumentationService.PostExceptionAsync(exception.ToString());
            }
            context.Result = result;
        }
    }
}
