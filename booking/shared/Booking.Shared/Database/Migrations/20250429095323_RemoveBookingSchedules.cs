using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBookingSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingSchedules",
                table: "Booking");

            migrationBuilder.AlterColumn<ICollection<BookingSchedule>>(
                name: "Schedules",
                table: "Booking",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<BookingSchedule>),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<BookingSchedule>>(
                name: "Schedules",
                table: "Booking",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(ICollection<BookingSchedule>),
                oldType: "jsonb");

            migrationBuilder.AddColumn<BookingSchedules>(
                name: "BookingSchedules",
                table: "Booking",
                type: "jsonb",
                nullable: false);
        }
    }
}
