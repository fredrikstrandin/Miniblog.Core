using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Core.Attribute;

namespace Miniblog.Core.Controllers
{
    [ServiceFilter(typeof(TenantAttribute))]
    public class TenantController : Controller
    {
        public IActionResult Index()
        {
            string str = HttpContext.Request.Host.Value;
            return View();
        }
    }
}