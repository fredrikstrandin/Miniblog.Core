using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Multiblog.Core.Repository;
using Multiblog.Model;

namespace Multiblog.Core.Attribute
{
    public class TenantAttribute : ActionFilterAttribute
    {
        private readonly IBlogRepository _blogRepository;

        public TenantAttribute(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var fullAddress = actionExecutingContext.HttpContext?.Request?
                .Headers?["Host"].ToString()?.Split('.');

            if (fullAddress.Length < 2 || (fullAddress.Length == 3 && fullAddress[0].ToLower() == "www"))
            {   
                base.OnActionExecuting(actionExecutingContext);
            }
            else
            {
                var subdomain = fullAddress[0];
                BlogItem tenant = _blogRepository.FindBlogAsync(subdomain.Normalize()).GetAwaiter().GetResult();

                if (tenant != null)
                {
                    actionExecutingContext.RouteData.Values.Add("tenant", tenant);
                    base.OnActionExecuting(actionExecutingContext);
                }
                else
                {
                    actionExecutingContext.Result = new StatusCodeResult(404);
                    base.OnActionExecuting(actionExecutingContext);
                }
            }
        }
    }
}
