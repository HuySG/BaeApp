using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public Guid userId { get; set; }
        public Guid TaskItemId { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsRead { get; set; }
    }
}
