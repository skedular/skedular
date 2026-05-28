using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedByMonthDayAndBySetPositionFromRecurringBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByMonthDay",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "BySetPosition",
                table: "RecurringBooking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ByMonthDay",
                table: "RecurringBooking",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BySetPosition",
                table: "RecurringBooking",
                type: "integer",
                nullable: true);
        }
    }
}
