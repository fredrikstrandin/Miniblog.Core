using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Venter.Utilities.Exception;
using Venter.Utilities.Middleware.RequestLogging;
using Venter.Utilities.Model;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.ApplicationInsights;

namespace Venter.Utilities.Filter
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly IRequestSave _requestSave;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IRequestSave requestSave)
        {
            _logger = logger;
            _requestSave = requestSave;
        }

        public void OnException(ExceptionContext context)
        {
            ApiError apiError;

            _requestSave.ExceptionAsync(context);

            _logger.LogDebug(context.Exception.Message);

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                _logger.LogDebug("Inloggad");
            }
            else
            {
                _logger.LogDebug("inte inloggad");
            }

            if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access") { StatusCode = 401 };
                context.HttpContext.Response.StatusCode = 401;
                _logger.LogWarning("Unauthorized Access in Controller Filter.");
            }
            else if (context.Exception is WebMessageException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as WebMessageException;
                apiError = new ApiError(ex.Message) { StatusCode = ex.StatusCode };

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is MongoWriteException)
            {
                apiError = new ApiError("Internal Server Error");
                context.HttpContext.Response.StatusCode = 500;
                _logger.LogWarning(context.Exception.Message);
            }
            else
            {
                apiError = new ApiError("Internal Server Error");
                context.HttpContext.Response.StatusCode = 500;
                _logger.LogWarning("Internal Server Error in Controller Filter.");
            }
            
            if (context.HttpContext.User.Claims.Where(x => x.Type == "role").Where(x => x.Value == "DEVELOPER").FirstOrDefault() != null)
            {
                apiError.Detail = context.Exception.Message;
            }

            Dictionary<string, string> dicInfo = new Dictionary<string, string>();
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                dicInfo.Add("Authenticated", "true");

                foreach (var claim in context.HttpContext.User.Claims)
                {
                    if (dicInfo.ContainsKey(claim.Type))
                    {
                        dicInfo[claim.Type] += $", {claim.Value}";
                    }
                    else
                    {
                        dicInfo.Add(claim.Type, claim.Value);
                    }
                }

            }
            else
            {
                dicInfo.Add("Authenticated", "false");
            }

            TelemetryClient telemetryClient = new TelemetryClient();
            telemetryClient.TrackException(context.Exception, dicInfo);

            context.Exception = null;

            // always return a JSON result
            context.Result = new JsonResult(apiError);
        }
    }
}
