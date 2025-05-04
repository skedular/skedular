using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAMountAndCurrencyToBookingCheckoutSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotal",
                table: "BookingCheckoutSession",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BookingCheckoutSession",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountTotal",
                table: "BookingCheckoutSession");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BookingCheckoutSession");
        }
    }
}
