using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFromAndUntilToRecurringBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_Until",
                table: "RecurringBooking");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "RecurringBooking",
                newName: "StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_RecurringBooking_Start",
                table: "RecurringBooking",
                newName: "IX_RecurringBooking_StartDate");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Until",
                table: "RecurringBooking",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "RecurringBooking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "RecurringBooking",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "From",
                table: "RecurringBooking",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldDefaultValue: "PRIVATE");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_Channel",
                table: "RecurringBooking",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_EndDate",
                table: "RecurringBooking",
                column: "EndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_Channel",
                table: "RecurringBooking");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_EndDate",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "From",
                table: "RecurringBooking");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "RecurringBooking",
                newName: "Start");

            migrationBuilder.RenameIndex(
                name: "IX_RecurringBooking_StartDate",
                table: "RecurringBooking",
                newName: "IX_RecurringBooking_Start");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Until",
                table: "RecurringBooking",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "PRIVATE",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_Until",
                table: "RecurringBooking",
                column: "Until");
        }
    }
}
