using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationOverrideMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationOverrideReason",
                table: "MarketplaceBookingSubscription",
                type: "character varying(100000)",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CancellationPolicyOverridden",
                table: "MarketplaceBookingSubscription",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CancellationOverrideReason",
                table: "Booking",
                type: "character varying(100000)",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CancellationPolicyOverridden",
                table: "Booking",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationOverrideReason",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "CancellationPolicyOverridden",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "CancellationOverrideReason",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CancellationPolicyOverridden",
                table: "Booking");
        }
    }
}
