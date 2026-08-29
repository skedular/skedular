using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdToMarketplacePurchaseHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_EventId",
                table: "MarketplacePurchaseHistory",
                column: "EventId",
                unique: true,
                filter: "\"EventId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_EventId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "MarketplacePurchaseHistory");
        }
    }
}
