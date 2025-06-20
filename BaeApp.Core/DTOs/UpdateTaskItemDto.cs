using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class UpdateTaskItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; } = 1;

        public string Status { get; set; } // Ví dụ: "Pending", "Completed", ...

        public List<Guid> CategoryIds { get; set; }
    }
}
