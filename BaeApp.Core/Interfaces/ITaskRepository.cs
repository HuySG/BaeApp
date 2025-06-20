using BaeApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface ITaskRepository
    {
        // Tạo Mới Task
        Task AddAsync(TaskItem taskItem);

        // Lấy danh sách tất cả task của một user ( hoặc tất cả với role Admin )

        Task<List<TaskItem>> GetByCreatorIdAsync(Guid creatorId);

        // Lấy chi tiết TaskItem theo Id

        Task<TaskItem> GetByIdAsync(Guid taskItemId);

        // Cập nhật TaskItem

        Task UpdateAsync(TaskItem taskItem);

        //Xóa Task Item

        Task DeleteAsync(TaskItem taskItem);

    }
}
