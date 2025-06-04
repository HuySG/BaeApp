using BaeApp.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync (RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
        Task<UserDto> GetByIdAsync(Guid userId);
        Task<bool> ChangeRoleAsync(Guid userId, string newRole);
    }
}
