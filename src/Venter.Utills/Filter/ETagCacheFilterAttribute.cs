using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Multiblog.Utilities.Filter
{
    public class ETagCheckFilterAttribute : Attribute, IResourceFilter
    {
        private readonly string _collection;
        private readonly string _key;
        
        public ETagCheckFilterAttribute(string collection, string key)
        {
            _collection = collection;
            _key = key;
        }

        public async void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.RouteData.Values.ContainsKey("id"))
            {
                string id = (string)context.RouteData.Values["id"];

                long eTag = await ETagRepository.GetETagAsync(_collection, _key, id);

                if (eTag == 0)
                {
                    context.Result = new NotFoundResult();
                }
                else
                {
                    if (context.HttpContext.Request.Headers.ContainsKey("If-None-Match") && context.HttpContext.Request.Headers["If-None-Match"] == eTag.ToString())
                    {
                        // not modified
                        context.Result = new StatusCodeResult(304);
                    }

                    if (!context.HttpContext.Response.HasStarted && !context.HttpContext.Response.Headers.ContainsKey("ETag"))
                        context.HttpContext.Response.Headers.Add("ETag", new[] { eTag.ToString() });
                }
            }
        }


        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
