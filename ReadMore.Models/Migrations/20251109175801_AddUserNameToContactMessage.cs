using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadMore.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToContactMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ContactMessages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContactMessages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e3886e3-9279-4c46-9565-4c57b341129d", "AQAAAAIAAYagAAAAEEv5Z+dgsQqsT5QxSZgz1EMW+TRzGvT3lsktzCz3LIkQZS2y6VP4NrO+LqvICDs+ow==", "55549360-c2b2-44ea-9705-ad7cb6118860" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_UserId",
                table: "ContactMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_UserId",
                table: "ContactMessages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_UserId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_UserId",
                table: "ContactMessages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "41da4854-f62c-45ec-95b1-7ae6e29f6403", "AQAAAAIAAYagAAAAEEtI8j8ZS6vwcarkvkXwN89mM+k88auoz+PWlPWL6+/2DVH8oDHFSKAbjX9b3jpPPw==", "d12b7d74-473f-424e-8cc9-6e39cb8868b4" });
        }
    }
}
