using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
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

            migrationBuilder.DropColumn(
                name: "MaxBookingSpreadDays",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "RecurrenceWindowDays",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "RequireConsecutiveDays",
                table: "Product");
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
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequireConsecutiveDays",
                table: "ProductVersion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxBookingSpreadDays",
                table: "Product",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceWindowDays",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequireConsecutiveDays",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
