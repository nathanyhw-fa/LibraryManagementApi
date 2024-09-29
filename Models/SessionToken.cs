using System;

namespace LibraryManagementSystem.Models
{
    public class SessionToken
    {
        public Guid SessionId { get; set; }  // Primary Key
        public Guid UserId { get; set; }  // Foreign Key to UserInfo

        public string Token { get; set; }
        public int Status { get; set; }  // Represents session status (e.g., active, expired)
        public DateTime CreatedDateUTC { get; set; }
        public DateTime LastUpdatedDateUTC { get; set; }
        public DateTime ExpiryDateUTC { get; set; }

        // Navigation property to UserInfo
        public UserInfo UserInfo { get; set; }
    }
}
