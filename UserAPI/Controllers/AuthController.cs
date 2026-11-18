using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models;
using UserAPI.DTOs;

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
        public async Task<ActionResult<UserResponse>> Register(RegisterRequest registerRequest)
        {
            var email = registerRequest.Email.ToLower();
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Email = registerRequest.Email,
                DisplayName = registerRequest.DisplayName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { email = user.Email }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            return NoContent();
        }

    }
}
