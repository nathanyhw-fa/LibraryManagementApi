using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class BookInfo
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public string Synopsis { get; set; }
        public string Publisher { get; set; }
        public DateOnly PublicationDate { get; set; }
        public string Edition { get; set; }
        public int NumberOfPages { get; set; }
        public string Language { get; set; }
        public string? Isbn { get; set; }
        public int Status { get; set; }

        public Guid CreatedByUserId { get; set; }
        public Guid LastUpdatedByUserId { get; set; }
        public DateTime CreatedDateUTC { get; set; }
        public DateTime LastUpdatedDateUTC { get; set; }

        public UserInfo CreatedByUserInfo { get; set; }
        public UserInfo LastUpdatedByUserInfo { get; set; }

        public ICollection<ReserveBook> ReserveBooks { get; set; }
        public ICollection<BookTracking> BookTrackings { get; set; }

        
    }
}
