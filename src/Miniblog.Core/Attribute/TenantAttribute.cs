using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Miniblog.Core.Repository.MongoDB;
using Miniblog.Core.Repository.MongoDB.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Core.Attribute
{
    public class TenantAttribute : ActionFilterAttribute
    {
        private readonly MongoDBContext _context;

        public TenantAttribute(IOptions<MongoDbDatabaseSetting> _dbStetting)
        {
            _context = new MongoDBContext(_dbStetting.Value);
        }

        public async override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var fullAddress = actionExecutingContext.HttpContext?.Request?
                .Headers?["Host"].ToString()?.Split('.');
            if (fullAddress.Length < 2)
            {
                actionExecutingContext.Result = new StatusCodeResult(404);
                base.OnActionExecuting(actionExecutingContext);
            }
            else
            {
                var subdomain = fullAddress[0];
                var tenant = await  _context.BlogEntityCollection.Find(x => x.SubDomainNormalize == subdomain)
                    .FirstOrDefaultAsync();

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
