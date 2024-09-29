using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInfos_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookInfos",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Synopsis = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Edition = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    NumberOfPages = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookInfos", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_BookInfos_UserInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookInfos_UserInfos_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberInfos",
                columns: table => new
                {
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInfos", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_MemberInfos_UserInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberInfos_UserInfos_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionTokens",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ExpiryDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTokens", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_SessionTokens_UserInfos_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookTrackings",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookTrackings", x => x.TrackingId);
                    table.ForeignKey(
                        name: "FK_BookTrackings_BookInfos_BookId",
                        column: x => x.BookId,
                        principalTable: "BookInfos",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookTrackings_MemberInfos_MemberId",
                        column: x => x.MemberId,
                        principalTable: "MemberInfos",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookTrackings_UserInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookTrackings_UserInfos_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReserveBooks",
                columns: table => new
                {
                    ReserveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReservedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveBooks", x => x.ReserveId);
                    table.ForeignKey(
                        name: "FK_ReserveBooks_BookInfos_BookId",
                        column: x => x.BookId,
                        principalTable: "BookInfos",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReserveBooks_MemberInfos_MemberId",
                        column: x => x.MemberId,
                        principalTable: "MemberInfos",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReserveBooks_UserInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReserveBooks_UserInfos_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberPayments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfOverdue = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPayments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_MemberPayments_BookTrackings_TrackingId",
                        column: x => x.TrackingId,
                        principalTable: "BookTrackings",
                        principalColumn: "TrackingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberPayments_UserInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserInfos",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookInfos_CreatedByUserId",
                table: "BookInfos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookInfos_LastUpdatedByUserId",
                table: "BookInfos",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTrackings_BookId",
                table: "BookTrackings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTrackings_CreatedByUserId",
                table: "BookTrackings",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTrackings_LastUpdatedByUserId",
                table: "BookTrackings",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTrackings_MemberId",
                table: "BookTrackings",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_CreatedByUserId",
                table: "MemberInfos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_LastUpdatedByUserId",
                table: "MemberInfos",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPayments_CreatedByUserId",
                table: "MemberPayments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPayments_TrackingId",
                table: "MemberPayments",
                column: "TrackingId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveBooks_BookId",
                table: "ReserveBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveBooks_CreatedByUserId",
                table: "ReserveBooks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveBooks_LastUpdatedByUserId",
                table: "ReserveBooks",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveBooks_MemberId",
                table: "ReserveBooks",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionTokens_UserId",
                table: "SessionTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_RoleId",
                table: "UserInfos",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberPayments");

            migrationBuilder.DropTable(
                name: "ReserveBooks");

            migrationBuilder.DropTable(
                name: "SessionTokens");

            migrationBuilder.DropTable(
                name: "BookTrackings");

            migrationBuilder.DropTable(
                name: "BookInfos");

            migrationBuilder.DropTable(
                name: "MemberInfos");

            migrationBuilder.DropTable(
                name: "UserInfos");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
