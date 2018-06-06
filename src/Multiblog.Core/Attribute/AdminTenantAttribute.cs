using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core.Attribute
{
    public class AdminTenantAttribute : ActionFilterAttribute
    {
        private readonly UrlSettings _urlSetting;

        public AdminTenantAttribute(IOptions<UrlSettings> urlSetting)
        {
            _urlSetting = urlSetting.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var fullAddress = actionExecutingContext.HttpContext?.Request?
                .Headers?["Host"].ToString()?.Split('.');
            if (fullAddress.Length < _urlSetting.DotCountMin)
            {
                actionExecutingContext.Result = new StatusCodeResult(404);
                base.OnActionExecuting(actionExecutingContext);
            }
            else
            {
                var subdomain = fullAddress[0];
                //We got the subdomain value, next verify it from database and
                //inject the information to RouteContext
            }
        }
    }
}
