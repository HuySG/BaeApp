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
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _context;
        public async Task AddAsync(Reminder reminder)
        {
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Reminder reminder)
        {
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }

        public Task<Reminder> GetByIdAsync(Guid id)
        => _context.Reminders.FindAsync(id).AsTask();

        public async Task<List<Reminder>> GetPendingAsync(DateTime now)
        {
            return await _context.Reminders
                .Include(r => r.TaskItem)
                .Where(r => !r.isSent && r.RemindAt <= now)
                .ToListAsync();
        }

        public async Task UpdateAsync(Reminder reminder)
        {
            _context.Reminders.Update(reminder);
            await _context.SaveChangesAsync();
        }
    }
}
