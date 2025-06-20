using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class CreateTaskItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; } = 1;

        // Nếu muốn client chỉ cần truyền List<CategoryId> để gán category
        public List<Guid> CategoryIds { get; set; }
    }
}
