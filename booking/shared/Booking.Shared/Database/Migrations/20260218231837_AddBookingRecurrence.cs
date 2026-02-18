using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingRecurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingRecurrenceId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingRecurrence",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    ByWeekDays = table.Column<string>(type: "jsonb", nullable: false),
                    ByMonthDay = table.Column<int>(type: "integer", nullable: true),
                    BySetPosition = table.Column<int>(type: "integer", nullable: true),
                    EndType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OccurrenceCount = table.Column<int>(type: "integer", nullable: true),
                    SkippedDates = table.Column<string>(type: "jsonb", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRecurrence", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BookingRecurrenceId",
                table: "Booking",
                column: "BookingRecurrenceId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_CreatedAt",
                table: "BookingRecurrence",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_DeletedAt",
                table: "BookingRecurrence",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_EndType",
                table: "BookingRecurrence",
                column: "EndType");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_Frequency",
                table: "BookingRecurrence",
                column: "Frequency");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_ModifiedAt",
                table: "BookingRecurrence",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_Start",
                table: "BookingRecurrence",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrence_Until",
                table: "BookingRecurrence",
                column: "Until");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_BookingRecurrence_BookingRecurrenceId",
                table: "Booking",
                column: "BookingRecurrenceId",
                principalTable: "BookingRecurrence",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_BookingRecurrence_BookingRecurrenceId",
                table: "Booking");

            migrationBuilder.DropTable(
                name: "BookingRecurrence");

            migrationBuilder.DropIndex(
                name: "IX_Booking_BookingRecurrenceId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BookingRecurrenceId",
                table: "Booking");
        }
    }
}
