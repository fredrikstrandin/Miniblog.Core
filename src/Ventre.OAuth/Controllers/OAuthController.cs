using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Vivus.OAuth.Controllers
{
    [Produces("application/json")]
    [Route("api/OAuth")]
    public class OAuthController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { text = "Lyckat" });
        }
    }
}