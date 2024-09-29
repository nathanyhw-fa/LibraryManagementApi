using System;

namespace LibraryManagementSystem.Models
{
	public class ReserveBook
	{
		public Guid ReserveId { get; set; }  // Primary Key
		public Guid MemberId { get; set; }  // Foreign Key to MemberInfo
		public Guid BookId { get; set; }  // Foreign Key to BookInfo

		public int Status { get; set; }  // Reservation status (e.g., active, completed)
		public DateTime ReservedDateUTC { get; set; }
		public DateTime LastUpdatedDateUTC { get; set; }

		// Foreign Keys (navigation properties)
		public MemberInfo MemberInfo { get; set; }
		public BookInfo BookInfo { get; set; }

		// Created/Updated by UserInfo
		public Guid CreatedByUserId { get; set; }
		public Guid LastUpdatedByUserId { get; set; }

		public UserInfo CreatedByUserInfo { get; set; }
		public UserInfo LastUpdatedByUserInfo { get; set; }
	}
}
