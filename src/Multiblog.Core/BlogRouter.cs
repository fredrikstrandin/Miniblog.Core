using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core
{
    public class BlogRouter : IRouter
    {
        private IRouter _defaultRouter;

        public BlogRouter(IRouter defaultRouteHandler)
        {
            _defaultRouter = defaultRouteHandler;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _defaultRouter.GetVirtualPath(context);
        }

        public async Task RouteAsync(RouteContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            var path = context.HttpContext.Request.Path.Value.Split('/');

            // Look for the User-Agent Header and Check if the Request comes from a Mobile 
            //string action = "Index";
            //string controller = "";

            if (path.Length == 3 && path[1] == "blog")
            {
                context.RouteData.Values["controller"] = "Gallery";
                context.RouteData.Values["action"] = "Post";
                context.RouteData.Values["slug"] = path[2];                
            }

            //if (path.Length > 1)
            //{
            //    controller = path[1];
            //    if (path.Length > 2)
            //        action = path[2];
            //}

            //if (controller == "blog")
            //{
            //    context.RouteData.Values["controller"] = "Gallery";
            //    context.RouteData.Values["action"] = action;
            //}

            await _defaultRouter.RouteAsync(context);
        
        }
    }    
}
