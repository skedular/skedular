using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameMustBookConsecutiveDaysToRequireConsecutiveDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MustBookConsecutiveDays",
                table: "ProductVersion",
                newName: "RequireConsecutiveDays");

            migrationBuilder.RenameColumn(
                name: "MustBookConsecutiveDays",
                table: "Product",
                newName: "RequireConsecutiveDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequireConsecutiveDays",
                table: "ProductVersion",
                newName: "MustBookConsecutiveDays");

            migrationBuilder.RenameColumn(
                name: "RequireConsecutiveDays",
                table: "Product",
                newName: "MustBookConsecutiveDays");
        }
    }
}
