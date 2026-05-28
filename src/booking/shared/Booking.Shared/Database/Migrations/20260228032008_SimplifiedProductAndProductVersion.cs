using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedProductAndProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBookingSpreadDays",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "RecurrenceWindowDays",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "RequireConsecutiveDays",
                table: "ProductVersion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxBookingSpreadDays",
                table: "ProductVersion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceWindowDays",
                table: "ProductVersion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireConsecutiveDays",
                table: "ProductVersion",
                type: "boolean",
                nullable: true,
                defaultValue: false);
        }
    }
}
