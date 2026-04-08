using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceRefundAccountingProjection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountingProvider",
                table: "MarketplaceRefund",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalRefundId",
                table: "MarketplaceRefund",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalRefundNumber",
                table: "MarketplaceRefund",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "MarketplaceRefund",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastProcessedAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_AccountingProvider_ExternalRefundId",
                table: "MarketplaceRefund",
                columns: new[] { "AccountingProvider", "ExternalRefundId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_AccountingProvider_ExternalRefundId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "AccountingProvider",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ExternalRefundId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ExternalRefundNumber",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "LastProcessedAt",
                table: "MarketplaceRefund");
        }
    }
}
