using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddHostBookingCommissionAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HostCommissionAmount",
                table: "MarketplaceBooking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HostCommissionRatePercentage",
                table: "MarketplaceBooking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HostPayoutAmount",
                table: "MarketplaceBooking",
                type: "numeric(18,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostCommissionAmount",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "HostCommissionRatePercentage",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "HostPayoutAmount",
                table: "MarketplaceBooking");
        }
    }
}
