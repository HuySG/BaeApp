using BaeApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetByUserAsync(Guid userId);
        Task<Notification> GetByIdAsync(Guid id);
        Task UpdateAsync(Notification notification);
    }
}
