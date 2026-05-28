using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecurringBookingToRecurringBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_BookingRecurrence_BookingRecurrenceId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_BookingRecurrence_BookingRecurrenceId",
                table: "MarketplaceBooking");

            migrationBuilder.DropTable(
                name: "BookingRecurrence");

            migrationBuilder.RenameColumn(
                name: "BookingRecurrenceId",
                table: "MarketplaceBooking",
                newName: "RecurringBookingId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketplaceBooking_BookingRecurrenceId",
                table: "MarketplaceBooking",
                newName: "IX_MarketplaceBooking_RecurringBookingId");

            migrationBuilder.RenameColumn(
                name: "BookingRecurrenceId",
                table: "Booking",
                newName: "RecurringBookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_BookingRecurrenceId",
                table: "Booking",
                newName: "IX_Booking_RecurringBookingId");

            migrationBuilder.CreateTable(
                name: "RecurringBooking",
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
                    table.PrimaryKey("PK_RecurringBooking", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_CreatedAt",
                table: "RecurringBooking",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_DeletedAt",
                table: "RecurringBooking",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_EndType",
                table: "RecurringBooking",
                column: "EndType");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_Frequency",
                table: "RecurringBooking",
                column: "Frequency");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_ModifiedAt",
                table: "RecurringBooking",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_Start",
                table: "RecurringBooking",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_Until",
                table: "RecurringBooking",
                column: "Until");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_RecurringBooking_RecurringBookingId",
                table: "Booking",
                column: "RecurringBookingId",
                principalTable: "RecurringBooking",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_RecurringBooking_RecurringBookingId",
                table: "MarketplaceBooking",
                column: "RecurringBookingId",
                principalTable: "RecurringBooking",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_RecurringBooking_RecurringBookingId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_RecurringBooking_RecurringBookingId",
                table: "MarketplaceBooking");

            migrationBuilder.DropTable(
                name: "RecurringBooking");

            migrationBuilder.RenameColumn(
                name: "RecurringBookingId",
                table: "MarketplaceBooking",
                newName: "BookingRecurrenceId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketplaceBooking_RecurringBookingId",
                table: "MarketplaceBooking",
                newName: "IX_MarketplaceBooking_BookingRecurrenceId");

            migrationBuilder.RenameColumn(
                name: "RecurringBookingId",
                table: "Booking",
                newName: "BookingRecurrenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_RecurringBookingId",
                table: "Booking",
                newName: "IX_Booking_BookingRecurrenceId");

            migrationBuilder.CreateTable(
                name: "BookingRecurrence",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ByMonthDay = table.Column<int>(type: "integer", nullable: true),
                    BySetPosition = table.Column<int>(type: "integer", nullable: true),
                    ByWeekDays = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OccurrenceCount = table.Column<int>(type: "integer", nullable: true),
                    SkippedDates = table.Column<string>(type: "jsonb", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRecurrence", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_BookingRecurrence_BookingRecurrenceId",
                table: "MarketplaceBooking",
                column: "BookingRecurrenceId",
                principalTable: "BookingRecurrence",
                principalColumn: "Id");
        }
    }
}
