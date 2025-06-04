using BaeApp.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }    
        public string Email { get; set; }

        public string FullName { get; set; }

        public RoleType Role { get; set; } 
        public DateTime CreateAt { get; set; } 
    }
}
