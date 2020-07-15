using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auther.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProtectedController : ControllerBase
    {
        [HttpGet("MyInfo")]
        public IActionResult MyInfo()
        {
            if(!User.Identity.IsAuthenticated) { return Unauthorized("Not Authenticated"); }

            var identity = User.Identity as ClaimsIdentity;

            var userId = identity.Claims.FirstOrDefault(c => c.Type == "userid").Value;

            return Ok(new { userId });
        }
    }
}
