using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class TaskItemDto
    {
        public Guid TaskItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public Guid CreatorId { get; set; }
        public string CreatorFullName { get; set; } // nếu muốn hiển thị thông tin người tạo
        public List<CategoryDto> Categories { get; set; } // nếu muốn đính kèm category
    }
}
