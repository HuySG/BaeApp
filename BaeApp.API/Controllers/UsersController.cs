using BaeApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaeApp.API.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    [Authorize] // tất cả action trong controller này cần được có token hợp lệ
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userDto = await _userService.GetCurrentUserAsync(User);
            if (userDto == null)
            {
                return NotFound(new { error = "User Không Tồn Tại" });
            }
            return Ok(userDto);
        }

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var userDto = await _userService.GetByIdAsync(userId);
            if (userDto == null)
            {
                return NotFound(new { error = "User Không Tồn Tại" });
            }
            return Ok(userDto);

        }

        [HttpPatch("{userId:guid}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(Guid userId, [FromBody] string newRole)
        {
            try
            {
                var success = await _userService.ChangeRoleAsync(userId, newRole);
                if (!success)
                {
                    return NotFound(new { error = "User Không Tồn Tại" });

                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
