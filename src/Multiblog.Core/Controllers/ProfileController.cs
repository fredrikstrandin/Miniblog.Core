using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Multiblog.Core.Model;

namespace Multiblog.Core.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            UserView view = new UserView()
            {
                Title = "Profile for Fredrik Strandin",
                FirstName = "Fredrik",
                LastName = "Strandin"
            };

            return View(view);
        }
    }
}