using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Repository;
using Multiblog.Model;
using Multiblog.Service.Blog;
using Multiblog.Service.Interface;

namespace Multiblog.Core.Attribute
{
    public class TenantAttribute : ActionFilterAttribute
    {
        private readonly IBlogService _blogService;

        private readonly BlogSettings _blogSettings;


        public TenantAttribute(IBlogService blogService,
            IOptions<BlogSettings> blogSettings)
        {
            _blogService = blogService;
            _blogSettings = blogSettings.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var fullAddress = actionExecutingContext.HttpContext?.Request?
                .Headers?["Host"].ToString()?.Split('.');

            if (fullAddress.Length < 2 || (fullAddress.Length == 3 && fullAddress[0].ToLower() == "www"))
            {
                if (!_blogSettings.Multitenant)
                {
                    BlogItem tenant = _blogService.FindBlogAsync("www").GetAwaiter().GetResult();

                    if (tenant == null)
                    {
                        actionExecutingContext.Result = new NotFoundResult();
                    }
                    else
                    {
                        actionExecutingContext.RouteData.Values.Add("tenant", tenant);
                    }
                }
            }
            else
            {
                if (_blogSettings.Multitenant)
                {
                    var subdomain = fullAddress[0];
                    BlogItem tenant = _blogService.FindBlogAsync(subdomain.Normalize()).GetAwaiter().GetResult();

                    if (tenant == null)
                    {
                        actionExecutingContext.Result = new NotFoundResult();
                    }
                    else
                    {
                        actionExecutingContext.RouteData.Values.Add("tenant", tenant);
                    }
                }
                else
                {
                    actionExecutingContext.Result = new NotFoundResult();
                }
            }

            base.OnActionExecuting(actionExecutingContext);
        }
    }
}
