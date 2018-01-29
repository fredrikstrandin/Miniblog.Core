using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Venter.Utilities.Middleware.RequestLogging
{
    public interface IRequestSave
    {
        Task IncomingAsync(HttpContext context, RequestInfo info);
        Task OutgoingAsync(HttpContext context, int statusCode, long millisecond);
        Task ExceptionAsync(ExceptionContext context);
    }
}