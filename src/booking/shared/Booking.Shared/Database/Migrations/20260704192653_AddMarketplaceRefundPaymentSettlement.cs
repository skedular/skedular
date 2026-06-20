using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceRefundPaymentSettlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalPaymentRefundId",
                table: "MarketplaceRefund",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "MarketplaceRefund",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentRefundLastError",
                table: "MarketplaceRefund",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaymentRefundLastProcessedAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentRefundStatus",
                table: "MarketplaceRefund",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_PaymentProvider_ExternalPaymentRefundId",
                table: "MarketplaceRefund",
                columns: new[] { "PaymentProvider", "ExternalPaymentRefundId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_PaymentProvider_ExternalPaymentRefundId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ExternalPaymentRefundId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "PaymentRefundLastError",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "PaymentRefundLastProcessedAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "PaymentRefundStatus",
                table: "MarketplaceRefund");
        }
    }
}
