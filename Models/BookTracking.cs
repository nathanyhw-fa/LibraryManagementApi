using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class BookTracking
    {
        public Guid TrackingId { get; set; } 
        public Guid BookId { get; set; }  
        public Guid MemberId { get; set; }  
        public DateOnly BorrowDate { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string Status { get; set; }

        
        public Guid CreatedByUserId { get; set; }  
        public Guid LastUpdatedByUserId { get; set; } 

       
        public DateTime CreatedDateUTC { get; set; }  
        public DateTime LastUpdatedDateUTC { get; set; } 

        public UserInfo CreatedByUserInfo { get; set; }  
        public UserInfo LastUpdatedByUserInfo { get; set; }  

        public BookInfo BookInfo { get; set; }
        public MemberInfo MemberInfo { get; set; } 

        public ICollection<MemberPayment> MemberPayments { get; set; }  
    }
}
