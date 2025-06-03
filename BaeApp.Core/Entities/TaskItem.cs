using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Entities
{
    public class TaskItem
    {
        public Guid TaskItemId { get; set; }        // PK
        public string Title { get; set; }           // NOT NULL, varchar(200)
        public string Description { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; } = 1;
        public string Status { get; set; } = "Pending";
        public Guid CreatorId { get; set; } // PK User.UserId

        // Navigation properties

        public User Creator { get; set; }
        public ICollection<TaskCategory> TaskCategories { get; set; }

        public ICollection<Reminder> Reminders { get; set; }

        public ICollection<Notification> Notifications { get; set; }

    }
}
