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
        public object MyInfo() => new 
        {
            id = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
            name = User.Claims.First(c => c.Type == ClaimTypes.Name).Value,
        };
    }
}
