using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BaeApp.Core.DTOs;
using BaeApp.Core.Entities;
using BaeApp.Core.Entities.Enums;
using BaeApp.Core.Interfaces;
using BaeApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace BaeApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Kiểm tra email đã tồn tại
            var existing = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existing != null)
                throw new Exception("Email đã được đăng ký.");

            // 2. Hash mật khẩu bằng BCrypt
            var hashedPwd = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = registerDto.Email,
                PasswordHash = hashedPwd,
                FullName = registerDto.FullName,
                Role = RoleType.Member,
                CreateAt = DateTime.UtcNow
            };

            // 3. Lưu vào database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 4. Sinh JWT token
            var token = GenerateJwtToken(user);

            // 5. Tạo DTO trả về
            var userDto = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreateAt = user.CreateAt
            };

            return new AuthResponseDto
            {
                Token = token,
                User = userDto
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // 1. Lấy user theo email
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
                throw new Exception("Email không tồn tại.");

            // 2. So sánh mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new Exception("Mật khẩu không chính xác.");

            // 3. Sinh token lại
            var token = GenerateJwtToken(user);

            // 4. Tạo DTO trả về
            var userDto = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreateAt = user.CreateAt
            };

            return new AuthResponseDto
            {
                Token = token,
                User = userDto
            };
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            // Lấy danh sách tất cả claim có Type = ClaimTypes.NameIdentifier
            var nameIdClaims = userPrincipal.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier);

            // Chọn claim nào parse thành GUID thành công
            var userIdClaim = nameIdClaims
                .FirstOrDefault(c => Guid.TryParse(c.Value, out _));

            if (userIdClaim == null)
            {
                Console.WriteLine("[DEBUG] Không tìm thấy claim NameIdentifier dạng GUID");
                return null;
            }

            var userId = Guid.Parse(userIdClaim.Value);
            Console.WriteLine($"[DEBUG] Parsed userId = {userId}");

            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                Console.WriteLine($"[DEBUG] Không tìm thấy User nào có UserId = {userId}");
                return null;
            }

            Console.WriteLine($"[DEBUG] Tìm thấy User: {user.Email}");

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreateAt = user.CreateAt
            };
        }

        public async Task<UserDto> GetByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreateAt = user.CreateAt
            };
        }
   
        public async Task<bool> ChangeRoleAsync(Guid userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!Enum.TryParse<RoleType>(newRole, out var parsedRole))
                throw new Exception("Role không hợp lệ.");

            user.Role = parsedRole;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    
    }
}
