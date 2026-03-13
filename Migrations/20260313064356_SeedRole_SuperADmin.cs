using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HybridApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedRole_SuperADmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { "b74ddd14-6340-4840-95c2-db12554843e5", "1", "Super Admin role", "Super Admin", "SUPER ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5");
        }
    }
}
