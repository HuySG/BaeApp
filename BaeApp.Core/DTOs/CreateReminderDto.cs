using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class CreateReminderDto
    {
        [Required]
        public Guid TaskItemId { get; set; }

        [Required]
        public DateTime RemindAt { get; set; }

    }
}
