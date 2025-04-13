using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeBookingSchedulesMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<BookingSchedules>(
                name: "BookingSchedules",
                table: "Booking",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(BookingSchedules),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<BookingSchedules>(
                name: "BookingSchedules",
                table: "Booking",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(BookingSchedules),
                oldType: "jsonb");
        }
    }
}
