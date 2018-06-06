using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Multiblog.Repository.Database;
using Multiblog.Service;
using Multiblog.Service.Interface;

namespace Multiblog.OAuth.Controllers
{
    [Route("/[controller]")]
    public class TestDataController : Controller
    {
        private readonly ITestDataService _testDataService;
        private readonly IDatabaseToolRepository _databaseToolRepository;

        public TestDataController(ITestDataService testDataService,
            IDatabaseToolRepository databaseToolRepository)
        {
            _testDataService = testDataService;
            _databaseToolRepository = databaseToolRepository;
        }

        public async Task<IActionResult> Index()
        {
            await _databaseToolRepository.DeleteBlogStore();
            await _databaseToolRepository.CreateIndexAsync();

            await _testDataService.CreateBlogsAsync();

            return View();
        }
    }
}