using BaeApp.Core.DTOs;
using BaeApp.Core.Entities;
using BaeApp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Infrastructure.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _repo;
        private readonly ITaskRepository _TaskRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserRepository _userRepo;

        public ReminderService(IReminderRepository repo, ITaskRepository taskRepo, INotificationRepository notificationRepo, IUserRepository userRepo)
        {
            _repo = repo;
            _TaskRepo = taskRepo;
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;
        }

        private Guid GetUserId(ClaimsPrincipal user)
        {
            var claim = user.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier
                && Guid.TryParse(c.Value, out _));
            if (claim == null)
            {
                throw new Exception("Không tìm thấy claim userId hợp lệ.");
            }
            return Guid.Parse(claim.Value);
        }

        public async Task<ReminderDto> CreateAsync(CreateReminderDto dto, ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            var task = await _TaskRepo.GetByIdAsync(dto.TaskItemId);
            if (task == null || task.CreatorId != userId)
                throw new Exception("Task Không Tồn Tại Hoặc Không Có quyền");
            var r = new Reminder
            {
                ReminderId = Guid.NewGuid(),
                TaskItemId = dto.TaskItemId,
                RemindAt = dto.RemindAt,
                isSent = false,
            };
            await _repo.AddAsync(r);

            return new ReminderDto
            {
                ReminderId = r.ReminderId,
                TaskItemId = r.TaskItemId,
                RemindAt = r.RemindAt,
                IsSent = r.isSent,
            };
        }

        public async Task<bool> DeleteAsync(Guid reminderId, ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            var r = await _repo.GetByIdAsync(reminderId);
            var task = await _TaskRepo.GetByIdAsync(r.TaskItemId);
            if (r == null || task.CreatorId != userId)
            {
                return false;
            }
            await _repo.DeleteAsync(r);
            return true;
        }

        public async Task<List<ReminderDto>> GetUserRemindersAsync(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            var userTask = (await _TaskRepo.GetByCreatorIdAsync(userId))
                .Select(t => t.TaskItemId)
                .ToHashSet();
            var all = await _repo.GetPendingAsync(DateTime.UtcNow.AddYears(100));
            return all.Where(r => userTask.Contains(r.TaskItemId))
                .Select(r => new ReminderDto
                {
                    ReminderId = r.ReminderId,
                    TaskItemId = r.TaskItemId,
                    RemindAt = r.RemindAt,
                    IsSent = r.isSent,
                }).ToList();
        }

        public async Task ProcessPendingAsync()
        {
            var now = DateTime.UtcNow;
            var pendings = await _repo.GetPendingAsync(now);
            foreach (var r in pendings)
            {
                var task = r.TaskItem;
                var user = await _userRepo.GetByIdAsync(task.TaskItemId);
                if (user == null) continue;

                var notifi = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = user.UserId,
                    TaskItemId = task.TaskItemId,
                    Content = $"Nhắc bạn: {task.Title} lúc {r.RemindAt:yyyy-MM-dd HH:mm}",
                    CreateAt = DateTime.UtcNow,
                    IsRead = false,
                };
                await _notificationRepo.AddAsync(notifi);

                r.isSent = true;
                await _repo.UpdateAsync(r);
            }
        }
    }
}
