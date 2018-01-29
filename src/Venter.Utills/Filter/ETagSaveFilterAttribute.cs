using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Venter.Utilities.Filter
{
    public class ETagSaveFilterAttribute : Attribute, IResourceFilter
    {
        private readonly string _collection;
        private readonly string _key;

        public ETagSaveFilterAttribute(string collection, string key)
        {
            _collection = collection;
            _key = key;
        }
                
        public void OnResourceExecuting(ResourceExecutingContext context)
        {            
        }
        
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            if (context.HttpContext.Items.ContainsKey("If-None-Match") && (bool)context.HttpContext.Items["If-None-Match"])
            {
                long version;

                string eTag = context.HttpContext.Response.Headers["If-None-Match"];
                                
                if (long.TryParse(eTag, out version))
                {
                    string id = (string)context.RouteData.Values["id"];

                    //ETagRepository.SetETagAsync(_collection, _key, id, version);
                }
            }
        }
    }
}
