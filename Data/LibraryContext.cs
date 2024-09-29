using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;  // Reference to the models

namespace LibraryManagementSystem.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        // DbSets for each table using names as per your design
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<BookInfo> BookInfos { get; set; }
        public DbSet<MemberInfo> MemberInfos { get; set; }
        public DbSet<SessionToken> SessionTokens { get; set; }
        public DbSet<BookTracking> BookTrackings { get; set; }
        public DbSet<ReserveBook> ReserveBooks { get; set; }
        public DbSet<MemberPayment> MemberPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Info configuration
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId);

                // Configuring data types
                entity.Property(e => e.UserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.RoleId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.Username)
                      .HasColumnType("nvarchar(500)") 
                      .IsRequired();

                entity.Property(e => e.Password)
                      .HasColumnType("nvarchar(500)") 
                      .IsRequired();

                entity.Property(e => e.DisplayName)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.Phone)
                      .HasColumnType("nvarchar(500)") 
                      .IsRequired();

                entity.Property(e => e.CreatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                // Configuring relationship
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.UserInfos)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // User Role configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                // Configuring data types
                entity.Property(e => e.RoleId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.RoleName)
                      .HasColumnType("nvarchar(50)")
                      .IsRequired();
            });

            // Book Info configuration
            modelBuilder.Entity<BookInfo>(entity =>
            {
                entity.HasKey(e => e.BookId);

                // Define data types
                entity.Property(e => e.BookId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.Title)
                      .HasColumnType("nvarchar(500)")  
                      .IsRequired();

                entity.Property(e => e.Genre)
                      .HasColumnType("nvarchar(500)") 
                      .IsRequired();

                entity.Property(e => e.Author)
                      .HasColumnType("nvarchar(500)") 
                      .IsRequired();

                entity.Property(e => e.Synopsis)
                      .HasColumnType("nvarchar(500)"); 

                entity.Property(e => e.Publisher)
                      .HasColumnType("nvarchar(500)") 
                      .IsRequired();

                entity.Property(e => e.PublicationDate)
                      .HasColumnType("date") 
                      .IsRequired();

                entity.Property(e => e.Edition)
                      .HasColumnType("nvarchar(500)")  
                      .IsRequired();

                entity.Property(e => e.NumberOfPages)
                      .HasColumnType("int")
                      .IsRequired();

                entity.Property(e => e.Language)
                      .HasColumnType("nvarchar(500)")  
                      .IsRequired();

                entity.Property(e => e.Isbn)
                      .HasColumnType("nvarchar(500)");  

                entity.Property(e => e.Status)
                      .HasColumnType("int")
                      .IsRequired();

                entity.Property(e => e.CreatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.CreatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                // Configuring relationship
                entity.HasOne(e => e.CreatedByUserInfo)
                      .WithMany(u => u.CreatedBooks)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);  

                entity.HasOne(e => e.LastUpdatedByUserInfo)
                      .WithMany(u => u.UpdatedBooks)
                      .HasForeignKey(e => e.LastUpdatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);  
            });

            // Member Info configuration
            modelBuilder.Entity<MemberInfo>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                // Define data types
                entity.Property(e => e.MemberId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.FullName)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.Phone)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.CreatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.CreatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                // Configuring Relationship
                entity.HasOne(e => e.CreatedByUserInfo)
                      .WithMany(u => u.CreatedMembers)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(e => e.LastUpdatedByUserInfo)
                      .WithMany(u => u.UpdatedMembers)
                      .HasForeignKey(e => e.LastUpdatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete
            });

            // Session Token configuration
            modelBuilder.Entity<SessionToken>(entity =>
            {
                entity.HasKey(e => e.SessionId);

                // Define data types 
                entity.Property(e => e.SessionId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.UserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.Token)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasColumnType("int")
                      .IsRequired();

                entity.Property(e => e.CreatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.ExpiryDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                // Configuring relationship
                entity.HasOne(e => e.UserInfo)
                      .WithMany(u => u.SessionTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Book Tracking configuration
            modelBuilder.Entity<BookTracking>(entity =>
            {
                entity.HasKey(e => e.TrackingId);

                // Define data types
                entity.Property(e => e.TrackingId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.BookId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.MemberId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.BorrowDate)
                      .HasColumnType("date")
                      .IsRequired();

                entity.Property(e => e.DueDate)
                      .HasColumnType("date")
                      .IsRequired();

                entity.Property(e => e.ReturnDate)
                      .HasColumnType("date")
                      .IsRequired(false);  

                entity.Property(e => e.Status)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.CreatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.CreatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                // Configuring relationship
                entity.HasOne(e => e.MemberInfo)
                      .WithMany(mi => mi.BookTrackings)
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BookInfo)
                      .WithMany(bi => bi.BookTrackings)
                      .HasForeignKey(e => e.BookId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUserInfo)
                      .WithMany(u => u.CreatedBookTrackings)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LastUpdatedByUserInfo)
                      .WithMany(u => u.UpdatedBookTrackings)
                      .HasForeignKey(e => e.LastUpdatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Reserve Book configuration
            modelBuilder.Entity<ReserveBook>(entity =>
            {
                entity.HasKey(e => e.ReserveId);

                // Define data types 
                entity.Property(e => e.ReserveId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.MemberId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.BookId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasColumnType("int")
                      .IsRequired();

                entity.Property(e => e.ReservedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.CreatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.LastUpdatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                // Configuring relationship
                entity.HasOne(e => e.MemberInfo)
                      .WithMany(mi => mi.ReserveBooks)
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BookInfo)
                      .WithMany(bi => bi.ReserveBooks)
                      .HasForeignKey(e => e.BookId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUserInfo)
                      .WithMany(u => u.CreatedReserveBooks)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LastUpdatedByUserInfo)
                      .WithMany(u => u.UpdatedReserveBooks)
                      .HasForeignKey(e => e.LastUpdatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Member Payment configuration
            modelBuilder.Entity<MemberPayment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                // Define data types 
                entity.Property(e => e.PaymentId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.TrackingId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                entity.Property(e => e.DayOfOverdue)
                      .HasColumnType("int")
                      .IsRequired();

                entity.Property(e => e.TotalAmount)
                      .HasColumnType("nvarchar(500)")
                      .IsRequired();

                entity.Property(e => e.CreatedDateUTC)
                      .HasColumnType("datetime2(7)")
                      .IsRequired();

                entity.Property(e => e.CreatedByUserId)
                      .HasColumnType("uniqueidentifier")
                      .IsRequired();

                // Configuring relationship
                entity.HasOne(e => e.BookTracking)
                      .WithMany(bt => bt.MemberPayments)
                      .HasForeignKey(e => e.TrackingId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUserInfo)
                      .WithMany(u => u.CreatedMemberPayments)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
