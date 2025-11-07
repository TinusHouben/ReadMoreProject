using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadMore.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddIsProcessedToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84dc7bf6-9d2b-49cf-a01d-2dd782a168ec", "AQAAAAIAAYagAAAAEFgvPh8hGoyyi6A4QKdYqZDQqRKgWdazFlQENgHDksp4FqKdxUTqML2sYKBxpFwRNA==", "81664764-e1be-4428-b57e-68454d740864" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aea717a4-6b6c-4859-9baf-0fa415b3f0fd", "AQAAAAIAAYagAAAAEFZxijlSIAUMF6SYFIx++6BcDuNcGfaBFC8kZW0rI9nzlM4pMgNBlKkoIWH8XnOA5w==", "6f7b1b3e-3c8e-4827-8368-c2d3790995aa" });
        }
    }
}
