using BaeApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    // Định nghĩa các phương thức CRUD cơ bản thao tác trực tiếp với table Users

    //Lưu ý: Các phương thức repository chỉ lo vấn đề đọc/ghi dữ liệu thô
    // không kèm logic nghiệp vụ ( hash mật khẩu, generate token,..)

    public interface IUserRepository
    {
        // Lấy entity User từ DB
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetByEmailAsync(string emails);

        // Thêm Mới User
        Task AddAsync(User user);

        // Cập nhật user ( thường dùng để đổi role hay đổi mật khẩu )
        Task UpdateAsync(User user);

        // Xóa User
        Task DeleteAsync(User user);
    }
}
