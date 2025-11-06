using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReadMore.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddIsbnToBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Comics",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Comics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Comics");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "Books");

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "IsDeleted", "Price", "Title" },
                values: new object[,]
                {
                    { 1, "Author A", false, 19.99m, "Book One" },
                    { 2, "Author B", false, 25.50m, "Book Two" }
                });

            migrationBuilder.InsertData(
                table: "Comics",
                columns: new[] { "Id", "IsDeleted", "NumberInSeries", "Price", "Series", "Title" },
                values: new object[] { 1, false, 1, 9.99m, "Series A", "Comic One" });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "BookId", "ComicId", "IsDeleted", "Quantity", "TotalPrice" },
                values: new object[] { 1, 1, null, false, 2, 39.98m });
        }
    }
}
