using BaeApp.Core.DTOs;
using BaeApp.Core.Entities;
using BaeApp.Core.Interfaces;
using BaeApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        // Cần để quản lý Transaction khi xử lý TaskCategories
        private readonly AppDbContext _context;
        // Cần lấy thông tin người dùng FullName nếu cần
        private readonly IUserRepository _userRepository;

        public TaskService(
            ITaskRepository taskRepository, AppDbContext context, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _context = context;
            _userRepository = userRepository;
        }
        // Tạo một method lấy userId từ ClaimsPrincipal
        private Guid GetUserIdFromClaims(ClaimsPrincipal userClaimsPrincipal)
        {
            // Lấy Tất Cả của Claim NameIdentifier
            var IdClaims = userClaimsPrincipal.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value);

            // Tìm Giá trị được parse thành GUID
            var guidValue = IdClaims.FirstOrDefault(val => Guid.TryParse(val, out _));
            if (guidValue == null)
            {
                throw new InvalidOperationException("Không tìm thấy UserId trong Claims");
            }
            return Guid.Parse(guidValue);
        }

        public async Task<TaskItemDto> CreateTaskAsync(ClaimsPrincipal userClaimsPrincipal, CreateTaskItemDto dto)
        {
            // tạo một biến userId lấy thông tin user thoe userId
            var userId = GetUserIdFromClaims(userClaimsPrincipal);

            // 1. Tạo entity TaskItem mới
            var taskEntity = new TaskItem
            {
                TaskItemId = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                CreateAt = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Status = "Pending",
                CreatorId = userId,
            };
            // 2. Nếu có CategoriesIds, gán vào TaskCategories
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                taskEntity.TaskCategories = new List<TaskCategory>();
                foreach (var catId in dto.CategoryIds)
                {
                    // Tạo entity trung gian
                    taskEntity.TaskCategories.Add(new TaskCategory
                    {
                        TaskItemId = taskEntity.TaskItemId,
                        CategoryId = catId
                    });
                }
            }
            // 3. Lưu vào DB
            await _taskRepository.AddAsync(taskEntity);

            // 4. Trả về DTO ( cần mapping từ TaskItem -> TaskItemDto )
            var user = await _userRepository.GetByIdAsync(userId);

            var taskDto = new TaskItemDto
            {
                TaskItemId = taskEntity.TaskItemId,
                Title = taskEntity.Title,
                Description = taskEntity.Description,
                CreateAt = taskEntity.CreateAt,
                DueDate = taskEntity.DueDate,
                Priority = taskEntity.Priority,
                Status = taskEntity.Status,
                CreatorId = userId,
                CreatorFullName = user?.FullName,
                Categories = taskEntity.TaskCategories?
                .Select(tc => new CategoryDto
                {
                    CategoryId = tc.CategoryId,
                    Name = tc.Category.Name
                }).ToList()
            };
            return taskDto;
        }

        public async Task<bool> DeleteTaskAsync(ClaimsPrincipal userClaimsPrincipal, Guid taskItemId)
        {
            var userId = GetUserIdFromClaims(userClaimsPrincipal);
            var task = await _taskRepository.GetByIdAsync(userId);
            if (task == null)
            {
                return false;
            }
            await _taskRepository.DeleteAsync(task);
            return true;
        }

        public async Task<TaskItemDto> GetTaskByIdAsync(ClaimsPrincipal userClaimsPrincipal, Guid taskItemId)
        {
            var userId = GetUserIdFromClaims(userClaimsPrincipal);
            var task = await _taskRepository.GetByIdAsync(taskItemId);
            if (task == null || task.CreatorId != userId)
            {
                return null;
            }
            return new TaskItemDto
            {
                TaskItemId = task.TaskItemId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                CreatorId = task.CreatorId,
                CreatorFullName = task.Creator.FullName,
                Categories = task.TaskCategories.Select(tc => new CategoryDto
                {
                    CategoryId = tc.CategoryId,
                    Name = tc.Category.Name,
                }).ToList(),
            };
        }

        public async Task<List<TaskItemDto>> GetUserTaskAsync(ClaimsPrincipal userClaimsPrincipal)
        {
            var userId = GetUserIdFromClaims(userClaimsPrincipal);
            var tasks = await _taskRepository.GetByCreatorIdAsync(userId);
            // Lỗi 
            var listDto = tasks.Select(t => new TaskItemDto
            {
                TaskItemId = t.TaskItemId,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Status = t.Status,
                CreatorId = t.CreatorId,
                CreatorFullName = t.Creator.FullName,
                Categories = t.TaskCategories.Select(tc => new CategoryDto
                {
                    CategoryId = tc.CategoryId,
                    Name = tc.Category.Name,
                }).ToList()
            }).ToList();

            return listDto;
        }

        public async Task<bool> UpdateTaskAsync(ClaimsPrincipal userClaimsPrincipal, Guid taskItemId, UpdateTaskItemDto dto)
        {
            var userId = GetUserIdFromClaims(userClaimsPrincipal);

            var task = await _taskRepository.GetByIdAsync(taskItemId);

            if (task == null || task.CreatorId != userId)
            {
                return false;
            }
            // 1. Cập Nhật Lại Các Trường Dữ Liệu
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Priority = dto.Priority;
            if (!string.IsNullOrEmpty(dto.Status)) task.Status = dto.Status;

            // 2. Xử lý category: Xóa hết và gán lại theo dto.CategoriesIds
            // (Đơn Giản nhất: xóa tất cả TaskCategories, sau đó thêm mới;
            // nếu muốn tối ưu, so sánh sự khác biệt giữ danh sách cũ và mới )
            var existingTCs = _context.TaskCategories.Where(tc => tc.TaskItemId == taskItemId);
            _context.TaskCategories.RemoveRange(existingTCs);

            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                task.TaskCategories = dto.CategoryIds
                    .Select(catId => new TaskCategory
                    {
                        TaskItemId = task.TaskItemId,
                        CategoryId = catId
                    }).ToList();
            }
            // 3. Gọi Repository.Update
            await _taskRepository.UpdateAsync(task);
            return true;

        }
    }
}
