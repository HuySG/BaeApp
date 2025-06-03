using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Entities
{
    public class Reminder
    {
        public Guid ReminderId { get; set; }    //PK
        public Guid TaskItemId { get; set; }    // FK -> TaskItem.TaskItemId
        public TaskItem TaskItem { get; set; }

        public DateTime RemindAt { get; set; }  // thời điểm nhắc
        public bool isSent { get; set; } = false;  // Đã gửi chưa
    }
}
