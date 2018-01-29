using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace Venter.Utilities.Middleware.RequestLogging
{
    public class RequestLoggerMiddleware_remove
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RequestLoggerOptions _options;
        private IRequestSave _requestSave;
        private Stopwatch _stopwatch;

        public RequestLoggerMiddleware_remove(RequestDelegate next, RequestLoggerOptions options, ILoggerFactory loggerFactory, IRequestSave requestSave)
        {
            _next = next;
            _options = options;
            _logger = loggerFactory.CreateLogger<RequestLoggerMiddleware_remove>();
            _requestSave = requestSave;
        }

        public async Task Invoke(HttpContext context)
        {
            _stopwatch = Stopwatch.StartNew();

            RequestInfo info = new RequestInfo()
            {
                Path = context.Request.Path,
                RemotePort = context.Request.HttpContext.Connection.RemotePort,
                RemoteIpAddress = context.Request.HttpContext.Connection.RemoteIpAddress,
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.Value,
                CreatedOn = DateTime.UtcNow
            };

            await _requestSave.IncomingAsync(context, info);

            await _next.Invoke(context);
            
            if (context.User.Identity.IsAuthenticated)
            {
                _logger.LogDebug(context.User.Identity.Name);
            }

            await _requestSave.OutgoingAsync(context, context.Response.StatusCode, _stopwatch.ElapsedMilliseconds);
        }
    }

    public class RequestLoggerOptions
    {
        public string Tempel { get; set; }
    }

    public static class RequestLoggerExtensions
    {
        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggerMiddleware_remove>();
        }

        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder, RequestLoggerOptions options)
        {
            return builder.UseMiddleware<RequestLoggerMiddleware_remove>(options);
        }
    }
}
