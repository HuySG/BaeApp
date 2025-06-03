using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; set; }                    // Pk

        public Guid UserId { get; set; }                            // FK -> User.UserId ( Người Nhận )
        public User User { get; set; }

        public Guid? TaskItemId { get; set; }                       // FK -> TaskItem.TaskItemId ( Nếu liên quan tới task )
        public TaskItem TaskItem { get; set; }                      
                
        public string Content { get; set; }                         // Nội Dung Thông Báo
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;   // Thời Gian
        public bool IsRead { get; set; } = false;                   // Đánh Dấu đọc hay chưa
    }
}
