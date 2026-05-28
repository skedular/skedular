using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedBookingBillingScheduleWithBillingMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingSchedule",
                table: "MarketplaceBooking");

            migrationBuilder.AddColumn<string>(
                name: "BillingMode",
                table: "MarketplaceBooking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_BillingMode",
                table: "MarketplaceBooking",
                column: "BillingMode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBooking_BillingMode",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "BillingMode",
                table: "MarketplaceBooking");

            migrationBuilder.AddColumn<DeprecatedType>(
                name: "BillingSchedule",
                table: "MarketplaceBooking",
                type: "jsonb",
                nullable: false);
        }
    }
}
