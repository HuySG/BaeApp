using BaeApp.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface IReminderService
    {
        Task<ReminderDto> CreateAsync(CreateReminderDto dto, ClaimsPrincipal user);
        Task<List<ReminderDto>> GetUserRemindersAsync(ClaimsPrincipal user);
        Task<bool> DeleteAsync(Guid reminderId, ClaimsPrincipal user);
        Task ProcessPendingAsync();
    }
}
