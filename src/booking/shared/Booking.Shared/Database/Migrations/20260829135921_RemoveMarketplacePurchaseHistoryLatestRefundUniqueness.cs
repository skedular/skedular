using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMarketplacePurchaseHistoryLatestRefundUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_LatestRefundId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_LatestRefundId",
                table: "MarketplacePurchaseHistory",
                column: "LatestRefundId",
                filter: "\"LatestRefundId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_LatestRefundId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_LatestRefundId",
                table: "MarketplacePurchaseHistory",
                column: "LatestRefundId",
                unique: true,
                filter: "\"LatestRefundId\" IS NOT NULL AND \"EventType\" IS NULL");
        }
    }
}
