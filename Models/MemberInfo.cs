using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
	public class MemberInfo
	{
		public Guid MemberId { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public Guid CreatedByUserId { get; set; }
		public DateTime CreatedDateUTC { get; set; }
		public Guid LastUpdatedByUserId { get; set; }
		public DateTime LastUpdatedDateUTC { get; set; }

		public UserInfo CreatedByUserInfo { get; set; }
		public UserInfo LastUpdatedByUserInfo { get; set; }

		public ICollection<ReserveBook> ReserveBooks { get; set; }  // Navigation property
		public ICollection<BookTracking> BookTrackings { get; set; }  // Navigation property
	}
}
