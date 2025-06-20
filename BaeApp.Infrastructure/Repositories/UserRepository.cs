using BaeApp.Core.Entities;
using BaeApp.Core.Interfaces;
using BaeApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        // AppDbContext được DI inject vào, cho phép repository thao tác với DB
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Thêm mới user vào DBset rồi gọi SaveChangesAysnc() để commit
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        // Xóa user và SaveChangesAsync() để commit

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        // lấy User theo emails 
        // AsNoTracking() cho biết chỉ đọc
        // Không theo dõi EF change tracking ( tăng hiệu năng khi chỉ đọc )
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        // Cập nhật user và SaveChangesAsync() để commit
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
