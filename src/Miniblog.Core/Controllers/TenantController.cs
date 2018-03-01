using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Multiblog.Core.Attribute;
using Multiblog.Model;

namespace Multiblog.Core.Controllers
{
    [ServiceFilter(typeof(TenantAttribute))]
    public class TenantController : Controller
    {
        public IActionResult Index()
        {
            foreach (var item in RouteData.Values)
            {
                string s = item.Key;
            } 

            return View();
        }
    }
}