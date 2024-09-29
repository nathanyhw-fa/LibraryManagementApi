using System;

namespace LibraryManagementSystem.Models
{
    public class MemberPayment
    {
        public Guid PaymentId { get; set; }  // Primary Key
        public Guid TrackingId { get; set; }  // Foreign Key to BookTracking

        public int DayOfOverdue { get; set; }
        public decimal TotalAmount { get; set; }  // The amount charged for overdue

        public DateTime CreatedDateUTC { get; set; }

        // Navigation property for the related book tracking record
        public BookTracking BookTracking { get; set; }

        // Created by UserInfo (user who recorded the payment)
        public Guid CreatedByUserId { get; set; }
        public UserInfo CreatedByUserInfo { get; set; }
    }
}
