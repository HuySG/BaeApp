using BaeApp.Core.DTOs;
using BaeApp.Core.Entities;
using BaeApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaeApp.API.Controllers
{
    [Route("api/v1/tasks")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskItemDto dto)
        {
            var taskDto = await _taskService.CreateTaskAsync(User, dto);
            return CreatedAtAction(nameof(GetTaskById), new
            {
                TaskItemId = taskDto.TaskItemId
            }, taskDto);
        }

        [HttpGet("{taskItemId:guid}")]
        public async Task<IActionResult> GetTaskById(Guid TaskItemId)
        {
            var taskDto = await _taskService.GetTaskByIdAsync(User, TaskItemId);
            if (taskDto == null)
            {
                return NotFound(new { error = "Task Không tồn tại hoặc không quyền xem" });
            }
            return Ok(taskDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserTask()
        {
            var list = await _taskService.GetUserTaskAsync(User);
            if (list == null) return NotFound(new { error = "Hiện Tại chưa có task !" });
            return Ok(list);
        }

        // Cập Nhật Task 
        [HttpPut("{taskItemId:guid}")]
        public async Task<IActionResult> UpdateTask(Guid taskItemId, [FromBody] UpdateTaskItemDto dto)
        {
            var success = await _taskService.UpdateTaskAsync(User, taskItemId, dto);
            if (!success)
            {
                return NotFound(new { error = "Task Không tồn tại hoặc không có quyền cập nhật" });

            }
            return NoContent();
        }

        // Delete Task
        [HttpDelete("{taskItemId:guid}")]
        public async Task<IActionResult> DeleteTask(Guid taskItemId)
        {
            var success = await _taskService.DeleteTaskAsync(User, taskItemId);
            if (!success)
            {
                return NotFound(new { error = "Task không tồn tại hoặc không có quyền xóa" });

            }
            return NoContent();
        }
    }
}
