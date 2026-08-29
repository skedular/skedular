using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBookingFailureCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountingCleanupStatus",
                table: "MarketplaceBookingFailure",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "NotRequired");

            migrationBuilder.AddColumn<int>(
                name: "CleanupAttemptCount",
                table: "MarketplaceBookingFailure",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CleanupLastAttemptAt",
                table: "MarketplaceBookingFailure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CleanupLeaseExpiresAt",
                table: "MarketplaceBookingFailure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CleanupLeaseOwner",
                table: "MarketplaceBookingFailure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CleanupLeaseRenewedAt",
                table: "MarketplaceBookingFailure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceReleaseStatus",
                table: "MarketplaceBookingFailure",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_ResourceReleaseStatus_CleanupLeas~",
                table: "MarketplaceBookingFailure",
                columns: new[] { "ResourceReleaseStatus", "CleanupLeaseExpiresAt", "FinalizedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingFailure_ResourceReleaseStatus_CleanupLeas~",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "AccountingCleanupStatus",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "CleanupAttemptCount",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "CleanupLastAttemptAt",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "CleanupLeaseExpiresAt",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "CleanupLeaseOwner",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "CleanupLeaseRenewedAt",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "ResourceReleaseStatus",
                table: "MarketplaceBookingFailure");
        }
    }
}
