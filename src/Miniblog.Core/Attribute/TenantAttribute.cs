using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miniblog.Core.Model.Setting;
using Miniblog.Core.Repository;
using Vivus.Model;

namespace Miniblog.Core.Attribute
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
