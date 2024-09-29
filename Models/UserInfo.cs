using System;

namespace LibraryManagementSystem.Models
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDateUTC { get; set; }
        public DateTime LastUpdatedDateUTC { get; set; }

        public UserRole Role { get; set; }

        public ICollection<BookInfo> CreatedBooks { get; set; }
        public ICollection<BookInfo> UpdatedBooks { get; set; }

        public ICollection<MemberInfo> CreatedMembers { get; set; }  
        public ICollection<MemberInfo> UpdatedMembers { get; set; }

        public ICollection<SessionToken> SessionTokens { get; set; }

        public ICollection<BookTracking> CreatedBookTrackings { get; set; }
        public ICollection<BookTracking> UpdatedBookTrackings { get; set; }

        public ICollection<ReserveBook> CreatedReserveBooks { get; set; } 
        public ICollection<ReserveBook> UpdatedReserveBooks { get; set; }

        public ICollection<MemberPayment> CreatedMemberPayments { get; set; }
    }
}
