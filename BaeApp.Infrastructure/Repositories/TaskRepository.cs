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
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskItem taskItem)
        {
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskItem)
        {
            _context?.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskItem>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await _context.TaskItems
                .Include(t => t.TaskCategories)
                .ThenInclude(tc => tc.Category)
                .Where(t => t.CreatorId == creatorId)
                .ToListAsync();
        }

        public async Task<TaskItem> GetByIdAsync(Guid taskItemId)
        {
            return await _context.TaskItems
                .Include(t => t.TaskCategories)
                .ThenInclude(tc => tc.Category)
                .SingleOrDefaultAsync(t => t.TaskItemId == taskItemId);
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            _context.TaskItems.Update(taskItem);
            await _context.SaveChangesAsync();
        }
    }
}
