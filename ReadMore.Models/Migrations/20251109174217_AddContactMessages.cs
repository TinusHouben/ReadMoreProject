using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadMore.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddContactMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "41da4854-f62c-45ec-95b1-7ae6e29f6403", "AQAAAAIAAYagAAAAEEtI8j8ZS6vwcarkvkXwN89mM+k88auoz+PWlPWL6+/2DVH8oDHFSKAbjX9b3jpPPw==", "d12b7d74-473f-424e-8cc9-6e39cb8868b4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84dc7bf6-9d2b-49cf-a01d-2dd782a168ec", "AQAAAAIAAYagAAAAEFgvPh8hGoyyi6A4QKdYqZDQqRKgWdazFlQENgHDksp4FqKdxUTqML2sYKBxpFwRNA==", "81664764-e1be-4428-b57e-68454d740864" });
        }
    }
}
