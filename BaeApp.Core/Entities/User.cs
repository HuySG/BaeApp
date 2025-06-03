using BaeApp.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Entities
{
    public class User
    {
        public Guid UserId { get; set; }                    //PK
        public string Email {  get; set; }                  // UNIQUE, NOT NULL
        public string PasswordHash { get; set; }            // NOT NULL
        public string FullName { get; set; }
        // Giá Trị Mặc Định là Member. Có Hai Giá trị là Member hoặc Admin
        public RoleType Role { get; set; } = RoleType.Member;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        //Navigation properties

        public ICollection<TaskItem> TasksCreated { get; set; }  
        public ICollection<Notification> Notifications { get; set; }



    }
}
