using BaeApp.Core.DTOs;
using BaeApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaeApp.API.Controllers
{
    // [ApiController] bật các tính năng liên quan đến API 
    // tự động validate [FromBody], trả lỗi 400 nếu model invalid
    // [Route("api/v1/auth")] Base route cho tất cả action bên trong


    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        //POST api/v1/auth/register
        [HttpPost("register")]
        [AllowAnonymous] // cho phép cả người chưa gọi token
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                // gọi _userService.RegisterAsync(dto) 
                // nếu thành công, trả 201 Created kèm object {token, user}
                var authResponse = await _userService.RegisterAsync(dto);
                return Created("", authResponse);
            }
            catch (Exception ex)
            {
                // Nếu lỗi, catch Exception và trả BadRequest 400 với message
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // gọi _userService.LoginAsync(dto)
                // nếu thành công trả 200
                var authResponse = await _userService.LoginAsync(dto);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                // lỗi email không tồn tại, password sai 
                // trả Unauthorized 401
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
