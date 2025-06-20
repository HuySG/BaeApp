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
        // Nhận RegisterDto, xử lý logic tạo user, hash mật khẩu, lưu vào DB
        // , sinh ra JWT, trả về AuthResponseDto
        Task<AuthResponseDto> RegisterAsync (RegisterDto registerDto);
        
        // Nhận LoginDto, xử lý kiểm tra email/password, nếu đúng thì sinh JWT
        // và trả AuthResponseDto
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        // Lấy ClaimsPrincipal ( đại diện User đã xác thực khi nhận token)
        // Từ đó tìm claim chứa NameIdentifier (GUID userId), rồi truy vấn 
        // DB lấy user, chuyển thành UserDto
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);

        // Trả về UserDto khi biết userId
        Task<UserDto> GetByIdAsync(Guid userId);

        // Chỉ Admin có quyền đổi role của user khác.
        // Lấy user theo userId, parse newRole thành enum RoleType, UpdateDB.
        Task<bool> ChangeRoleAsync(Guid userId, string newRole);
    }
}
