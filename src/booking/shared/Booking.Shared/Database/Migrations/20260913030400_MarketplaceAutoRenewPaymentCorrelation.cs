using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MarketplaceAutoRenewPaymentCorrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeInvoiceId",
                table: "MarketplaceBooking",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "MarketplaceBooking",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeInvoiceId",
                table: "EntitlementPurchase",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeInvoiceId",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "StripeInvoiceId",
                table: "EntitlementPurchase");
        }
    }
}
