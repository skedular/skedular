using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameMaxSpreadDaysToMaxBookingSpreadDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxSpreadDays",
                table: "ProductVersion",
                newName: "MaxBookingSpreadDays");

            migrationBuilder.RenameColumn(
                name: "MaxSpreadDays",
                table: "Product",
                newName: "MaxBookingSpreadDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxBookingSpreadDays",
                table: "ProductVersion",
                newName: "MaxSpreadDays");

            migrationBuilder.RenameColumn(
                name: "MaxBookingSpreadDays",
                table: "Product",
                newName: "MaxSpreadDays");
        }
    }
}
