using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Highdmin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureController : ControllerBase
    {
        [HttpGet("dashboard")]
        [Authorize(Policy = "Permission:menu:dashboard:Read")]
        public IActionResult GetDashboard()
        {
            return Ok("Access to dashboard granted.");
        }

        [HttpPost("users")]
        [Authorize(Policy = "Permission:menu:users:Create")]
        public IActionResult CreateUser()
        {
            return Ok("User created successfully.");
        }

        [HttpGet("users")]
        [Authorize(Policy = "Permission:menu:users:Read")]
        public IActionResult GetUsers()
        {
            return Ok("Access to users granted.");
        }
    }
}