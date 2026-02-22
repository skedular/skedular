using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBookingToBookingRecurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_Booking_BookingId",
                table: "MarketplaceBooking");

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "MarketplaceBooking",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddColumn<string>(
                name: "BookingRecurrenceId",
                table: "MarketplaceBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_BookingRecurrenceId",
                table: "MarketplaceBooking",
                column: "BookingRecurrenceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_BookingRecurrence_BookingRecurrenceId",
                table: "MarketplaceBooking",
                column: "BookingRecurrenceId",
                principalTable: "BookingRecurrence",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_Booking_BookingId",
                table: "MarketplaceBooking",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_BookingRecurrence_BookingRecurrenceId",
                table: "MarketplaceBooking");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_Booking_BookingId",
                table: "MarketplaceBooking");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBooking_BookingRecurrenceId",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "BookingRecurrenceId",
                table: "MarketplaceBooking");

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "MarketplaceBooking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_Booking_BookingId",
                table: "MarketplaceBooking",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
