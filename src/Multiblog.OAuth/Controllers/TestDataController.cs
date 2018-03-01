using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Multiblog.Service;

namespace Multiblog.Core.Controllers
{
    public class TestDataController : Controller
    {
        private readonly ITestDataService _testDataService;

        public TestDataController(ITestDataService testDataService)
        {
            _testDataService = testDataService;
        }
        public async Task<IActionResult> Index()
        {
            await _testDataService.CreateBlogsAsync();

            return View();
        }
    }
}