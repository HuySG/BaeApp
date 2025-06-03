using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Entities
{
    public class TaskCategory
    {
        // Đây là bảng trung gian cho quan hệ N - N giữa Taskitem và Category

        public Guid TaskItemId { get; set; }  // pk 1, FK -> TaskItem.TaskItemId
        public TaskItem TaskItem { get; set; }

        public Guid CategoryId { get; set; } // pk2, fk -> cagtegory.categoryId
        public Category Category { get; set; }
    }
}
