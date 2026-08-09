using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBookingModificationRecipientEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "MarketplaceBookingModificationNotificationDelivery",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "MarketplaceBookingModificationNotificationDelivery",
                type: "character varying(100000)",
                maxLength: 100000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "MarketplaceBookingModificationNotificationDelivery");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "MarketplaceBookingModificationNotificationDelivery");
        }
    }
}
