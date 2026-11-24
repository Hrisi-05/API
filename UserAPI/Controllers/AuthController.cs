using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.DTOs;
using UserAPI.Interfaces;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            return result.Success
            ? Created($"/users/{result.Data!.UserId}", result.Data)
            : Fail(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);
            if (!result.Success) return Fail(result);

            Response.Cookies.Append("access_token", result.Data!.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            return Ok(new { message = "Logged in" });

        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return NoContent();
        }
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request.Email);
            return Ok(new { message = "If the email exists, we have sent a link to reset your password." });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var ok = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            return ok
                ? Ok(new { message = "Password has been reset." })
                : BadRequest(new { message = "Invalid or expired link." });
        }
    }
}

