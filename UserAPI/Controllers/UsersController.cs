using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAPI.DTOs;
using UserAPI.Interfaces;


namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController, Authorize]

    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // get all users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetAllAsync());
        }

        // get user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : Fail(result);
        }

        // update user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateRequest updateRequest)
        {
            if (id != CurrentUserId) return Forbid();

            var result = await _userService.UpdateAsync(id, updateRequest);
            return result.Success ? Ok(result.Data) : Fail(result);
        }

        // delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id != CurrentUserId) return Forbid();

            var result = await _userService.DeleteAsync(id);
            return result.Success ? NoContent() : Fail(result);
        }
    }
}

