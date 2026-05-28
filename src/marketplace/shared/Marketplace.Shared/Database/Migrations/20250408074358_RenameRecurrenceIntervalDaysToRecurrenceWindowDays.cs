using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecurrenceIntervalDaysToRecurrenceWindowDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecurrenceIntervalDays",
                table: "ProductVersion",
                newName: "RecurrenceWindowDays");

            migrationBuilder.RenameColumn(
                name: "RecurrenceIntervalDays",
                table: "Product",
                newName: "RecurrenceWindowDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecurrenceWindowDays",
                table: "ProductVersion",
                newName: "RecurrenceIntervalDays");

            migrationBuilder.RenameColumn(
                name: "RecurrenceWindowDays",
                table: "Product",
                newName: "RecurrenceIntervalDays");
        }
    }
}
