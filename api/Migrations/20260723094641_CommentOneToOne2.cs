using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class CommentOneToOne2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "170d3421-1c0d-4e99-96d5-4043c3576f7d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23b16db1-72a6-46c2-bfe9-e4e530912f35");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "170d3421-1c0d-4e99-96d5-4043c3576f7d", "6bdf70e9-2056-466a-b49d-27f5f1bba5be", "Admin", "ADMIN" },
                    { "23b16db1-72a6-46c2-bfe9-e4e530912f35", "c6bd1b87-3869-453a-b82b-6e8a52f561ba", "User", "USER" }
                });
        }
    }
}
