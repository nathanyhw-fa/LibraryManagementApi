﻿// <auto-generated />
using System;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    [DbContext(typeof(LibraryContext))]
    partial class LibraryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LibraryManagementSystem.Models.BookInfo", b =>
                {
                    b.Property<Guid>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<string>("Edition")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Genre")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Isbn")
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("LastUpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<int>("NumberOfPages")
                        .HasColumnType("int");

                    b.Property<DateOnly>("PublicationDate")
                        .HasColumnType("date");

                    b.Property<string>("Publisher")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Synopsis")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("BookId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("LastUpdatedByUserId");

                    b.ToTable("BookInfos");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.BookTracking", b =>
                {
                    b.Property<Guid>("TrackingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly>("BorrowDate")
                        .HasColumnType("date");

                    b.Property<Guid>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<DateOnly>("DueDate")
                        .HasColumnType("date");

                    b.Property<Guid>("LastUpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly?>("ReturnDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("TrackingId");

                    b.HasIndex("BookId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("LastUpdatedByUserId");

                    b.HasIndex("MemberId");

                    b.ToTable("BookTrackings");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.MemberInfo", b =>
                {
                    b.Property<Guid>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("LastUpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("MemberId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("LastUpdatedByUserId");

                    b.ToTable("MemberInfos");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.MemberPayment", b =>
                {
                    b.Property<Guid>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<int>("DayOfOverdue")
                        .HasColumnType("int");

                    b.Property<string>("TotalAmount")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("TrackingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PaymentId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("TrackingId");

                    b.ToTable("MemberPayments");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.ReserveBook", b =>
                {
                    b.Property<Guid>("ReserveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LastUpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReservedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ReserveId");

                    b.HasIndex("BookId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("LastUpdatedByUserId");

                    b.HasIndex("MemberId");

                    b.ToTable("ReserveBooks");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.SessionToken", b =>
                {
                    b.Property<Guid>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<DateTime>("ExpiryDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<DateTime>("LastUpdatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("SessionTokens");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.UserInfo", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("LastUpdatedDateUTC")
                        .HasColumnType("datetime2(7)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserInfos");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.UserRole", b =>
                {
                    b.Property<Guid>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.BookInfo", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "CreatedByUserInfo")
                        .WithMany("CreatedBooks")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "LastUpdatedByUserInfo")
                        .WithMany("UpdatedBooks")
                        .HasForeignKey("LastUpdatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedByUserInfo");

                    b.Navigation("LastUpdatedByUserInfo");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.BookTracking", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.BookInfo", "BookInfo")
                        .WithMany("BookTrackings")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "CreatedByUserInfo")
                        .WithMany("CreatedBookTrackings")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "LastUpdatedByUserInfo")
                        .WithMany("UpdatedBookTrackings")
                        .HasForeignKey("LastUpdatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.MemberInfo", "MemberInfo")
                        .WithMany("BookTrackings")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BookInfo");

                    b.Navigation("CreatedByUserInfo");

                    b.Navigation("LastUpdatedByUserInfo");

                    b.Navigation("MemberInfo");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.MemberInfo", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "CreatedByUserInfo")
                        .WithMany("CreatedMembers")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "LastUpdatedByUserInfo")
                        .WithMany("UpdatedMembers")
                        .HasForeignKey("LastUpdatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedByUserInfo");

                    b.Navigation("LastUpdatedByUserInfo");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.MemberPayment", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "CreatedByUserInfo")
                        .WithMany("CreatedMemberPayments")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.BookTracking", "BookTracking")
                        .WithMany("MemberPayments")
                        .HasForeignKey("TrackingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BookTracking");

                    b.Navigation("CreatedByUserInfo");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.ReserveBook", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.BookInfo", "BookInfo")
                        .WithMany("ReserveBooks")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "CreatedByUserInfo")
                        .WithMany("CreatedReserveBooks")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "LastUpdatedByUserInfo")
                        .WithMany("UpdatedReserveBooks")
                        .HasForeignKey("LastUpdatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryManagementSystem.Models.MemberInfo", "MemberInfo")
                        .WithMany("ReserveBooks")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BookInfo");

                    b.Navigation("CreatedByUserInfo");

                    b.Navigation("LastUpdatedByUserInfo");

                    b.Navigation("MemberInfo");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.SessionToken", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.UserInfo", "UserInfo")
                        .WithMany("SessionTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("UserInfo");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.UserInfo", b =>
                {
                    b.HasOne("LibraryManagementSystem.Models.UserRole", "Role")
                        .WithMany("UserInfos")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.BookInfo", b =>
                {
                    b.Navigation("BookTrackings");

                    b.Navigation("ReserveBooks");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.BookTracking", b =>
                {
                    b.Navigation("MemberPayments");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.MemberInfo", b =>
                {
                    b.Navigation("BookTrackings");

                    b.Navigation("ReserveBooks");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.UserInfo", b =>
                {
                    b.Navigation("CreatedBookTrackings");

                    b.Navigation("CreatedBooks");

                    b.Navigation("CreatedMemberPayments");

                    b.Navigation("CreatedMembers");

                    b.Navigation("CreatedReserveBooks");

                    b.Navigation("SessionTokens");

                    b.Navigation("UpdatedBookTrackings");

                    b.Navigation("UpdatedBooks");

                    b.Navigation("UpdatedMembers");

                    b.Navigation("UpdatedReserveBooks");
                });

            modelBuilder.Entity("LibraryManagementSystem.Models.UserRole", b =>
                {
                    b.Navigation("UserInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
