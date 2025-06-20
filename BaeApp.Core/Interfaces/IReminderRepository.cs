using BaeApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
   public interface IReminderRepository
    {
        Task AddAsync(Reminder reminder);
        Task<List<Reminder>> GetPendingAsync(DateTime now);
        Task<Reminder> GetByIdAsync(Guid id);
        Task UpdateAsync (Reminder reminder);
        Task DeleteAsync (Reminder reminder);
    }
}
