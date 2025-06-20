using BaeApp.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface ITaskService
    {
        // Tạo Task mới trả về TaskItemDto

        Task<TaskItemDto> CreateTaskAsync(ClaimsPrincipal userClaimsPrincipal, CreateTaskItemDto dto);

        // Lấy danh sách task cho user hiện tại

        Task<List<TaskItemDto>> GetUserTaskAsync(ClaimsPrincipal userClaimsPrincipal);

        // Lấy Chi Tiết Task Theo Id

        Task<TaskItemDto> GetTaskByIdAsync(ClaimsPrincipal userClaimsPrincipal, Guid taskItemId);

        // Cập Nhật Task

        Task<bool> UpdateTaskAsync(ClaimsPrincipal userClaimsPrincipal, Guid taskItemId, UpdateTaskItemDto dto);

        // Xóa Task

        Task<bool> DeleteTaskAsync(ClaimsPrincipal userClaimsPrincipal, Guid taskItemId);
    }
}
