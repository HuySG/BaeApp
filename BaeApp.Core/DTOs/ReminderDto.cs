using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class ReminderDto
    {
        public Guid ReminderId { get; set; }
        public Guid TaskItemId { get; set; }
        public DateTime RemindAt { get; set; }
        public bool IsSent { get; set; }
    }
}
