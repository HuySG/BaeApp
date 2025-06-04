using BaeApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync (Guid userId);
        Task<User> GetByEmailAsync(string emails);
        Task AddAsync (User user);
        Task UpdateAsync (User user);
        Task DeleteAsync(User user);
    }
}
