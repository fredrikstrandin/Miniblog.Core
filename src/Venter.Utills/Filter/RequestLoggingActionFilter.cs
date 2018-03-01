using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Multiblog.Utilities.Filter
{
    public class RequestLoggingActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // do something before the action executes
            await next();
            // do something after the action executes
        }
    }
}
