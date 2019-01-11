using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Web.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        [Route("public")]
        public IActionResult Public()
        {
            return Ok(new { message = "Hello from a public API!" });
        }

        [Authorize]
        [Route("private")]
        public IActionResult Private()
        {
            return Ok(new { message = "Hello from a private API!" });
        }
    }
}