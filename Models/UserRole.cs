using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class UserRole
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<UserInfo> UserInfos { get; set; }
    }
}
