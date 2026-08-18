using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitlementPurchaseHistoryProjection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "MarketplacePurchaseHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreditQuantity",
                table: "MarketplacePurchaseHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntitlementStatus",
                table: "MarketplacePurchaseHistory",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GrantedQuantity",
                table: "MarketplacePurchaseHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory",
                column: "EntitlementPurchaseId",
                unique: true,
                filter: "\"EntitlementPurchaseId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_EntitlementPurchase_EntitlementP~",
                table: "MarketplacePurchaseHistory",
                column: "EntitlementPurchaseId",
                principalTable: "EntitlementPurchase",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_EntitlementPurchase_EntitlementP~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "CreditQuantity",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EntitlementStatus",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "GrantedQuantity",
                table: "MarketplacePurchaseHistory");
        }
    }
}
