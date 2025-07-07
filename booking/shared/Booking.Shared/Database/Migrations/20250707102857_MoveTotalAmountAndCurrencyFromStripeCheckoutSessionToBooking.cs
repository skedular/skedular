using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoveTotalAmountAndCurrencyFromStripeCheckoutSessionToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountTotal",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "StripeCheckoutSession");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Booking",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Currency",
                table: "Booking",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TotalAmount",
                table: "Booking",
                column: "TotalAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_Currency",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TotalAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Booking");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotal",
                table: "StripeCheckoutSession",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "StripeCheckoutSession",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);
        }
    }
}
