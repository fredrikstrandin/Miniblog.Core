using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multiblog.Core.Model;

namespace Multiblog.Core.Api
{
    [Produces("application/json")]
    [Route("api/Comment")]
    public class CommentController : Controller
    {
        [Authorize]
        public IActionResult Get()
        {
            return Ok(new MessageItem() { MessageId = Guid.NewGuid().ToString(), MessageText = "Detta är ett test" });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromForm]MessageItem item)
        {
            return Ok(new MessageRespons() { Success = true });
        }
    }
}