using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UserAPI.Interfaces;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwoFactorController : ApiControllerBase
    {
        readonly ITwoFactorService _twoFactorService;

        public TwoFactorController(ITwoFactorService twoFactorService)
        {
            _twoFactorService = twoFactorService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public record CodeRequest([Required] string Code);

        [HttpPost("setup")]
        public async Task<IActionResult> Setup()
        {
            var r = await _twoFactorService.SetupAsync(CurrentUserId);
            return r is null
                ? BadRequest(new { message = "2FA вече е включена. Първо я изключи." })
                : Ok(new { secret = r.Value.Secret, uri = r.Value.Uri });
        }

        [HttpPost("enable")]
        public async Task<IActionResult> Enable(CodeRequest request) =>
            await _twoFactorService.EnableAsync(CurrentUserId, request.Code)
                ? NoContent() : BadRequest(new { message = "Невалиден код." });

        [HttpPost("disable")]
        public async Task<IActionResult> Disable(CodeRequest request) =>
            await _twoFactorService.DisableAsync(CurrentUserId, request.Code)
                ? NoContent() : BadRequest(new { message = "Невалиден код." });
    }
}

