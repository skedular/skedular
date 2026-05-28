using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class DropProductPricingFromMarketplaceBookingSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPricing",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.AddColumn<string>(
                name: "MarketplaceBookingSubscriptionId",
                table: "MarketplaceBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_MarketplaceBookingSubscriptionId",
                table: "MarketplaceBooking",
                column: "MarketplaceBookingSubscriptionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_MarketplaceBookingSubscription_Marketpla~",
                table: "MarketplaceBooking",
                column: "MarketplaceBookingSubscriptionId",
                principalTable: "MarketplaceBookingSubscription",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_MarketplaceBookingSubscription_Marketpla~",
                table: "MarketplaceBooking");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBooking_MarketplaceBookingSubscriptionId",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "MarketplaceBookingSubscriptionId",
                table: "MarketplaceBooking");

            migrationBuilder.AddColumn<ProductPricing>(
                name: "ProductPricing",
                table: "MarketplaceBookingSubscription",
                type: "jsonb",
                nullable: false);
        }
    }
}
