using System;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBookingSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketplaceBookingSubscriptionId",
                table: "RecurringBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingSubscription",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    NextRenewalAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    CancelAtPeriodEnd = table.Column<bool>(type: "boolean", nullable: false),
                    ProductPricing = table.Column<ProductPricing>(type: "jsonb", nullable: false),
                    ProductVersionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscription_ProductVersion_ProductVersio~",
                        column: x => x.ProductVersionId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_MarketplaceBookingSubscriptionId",
                table: "RecurringBooking",
                column: "MarketplaceBookingSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_CancelledAt",
                table: "MarketplaceBookingSubscription",
                column: "CancelledAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_CreatedAt",
                table: "MarketplaceBookingSubscription",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_DeletedAt",
                table: "MarketplaceBookingSubscription",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_ModifiedAt",
                table: "MarketplaceBookingSubscription",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_NextRenewalAt",
                table: "MarketplaceBookingSubscription",
                column: "NextRenewalAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_ProductVersionId",
                table: "MarketplaceBookingSubscription",
                column: "ProductVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_StartedAt",
                table: "MarketplaceBookingSubscription",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_Status",
                table: "MarketplaceBookingSubscription",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBooking_MarketplaceBookingSubscription_Marketplace~",
                table: "RecurringBooking",
                column: "MarketplaceBookingSubscriptionId",
                principalTable: "MarketplaceBookingSubscription",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBooking_MarketplaceBookingSubscription_Marketplace~",
                table: "RecurringBooking");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingSubscription");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_MarketplaceBookingSubscriptionId",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "MarketplaceBookingSubscriptionId",
                table: "RecurringBooking");
        }
    }
}
