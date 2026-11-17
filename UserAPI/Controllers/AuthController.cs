using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly APIContext _context;

        public AuthController(APIContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(string email, string password, string displayName)
        {
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            return NoContent();
        }

    }
}
