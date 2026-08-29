using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplacePurchaseHistoryEventIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancellationEffectiveAt",
                table: "MarketplacePurchaseHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancellationRequestedAt",
                table: "MarketplacePurchaseHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EventAmount",
                table: "MarketplacePurchaseHistory",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EventAutoRenew",
                table: "MarketplacePurchaseHistory",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EventCancelAtPeriodEnd",
                table: "MarketplacePurchaseHistory",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventCreditQuantity",
                table: "MarketplacePurchaseHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventCurrency",
                table: "MarketplacePurchaseHistory",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventEntitlementStatus",
                table: "MarketplacePurchaseHistory",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EventIsDeleted",
                table: "MarketplacePurchaseHistory",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventReason",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100000)",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventRemainingCreditQuantity",
                table: "MarketplacePurchaseHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventSubscriptionStatus",
                table: "MarketplacePurchaseHistory",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "MarketplacePurchaseHistory",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OccurredAt",
                table: "MarketplacePurchaseHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousPaymentStatus",
                table: "MarketplacePurchaseHistory",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousRefundStatus",
                table: "MarketplacePurchaseHistory",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RecordedAt",
                table: "MarketplacePurchaseHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundStatus",
                table: "MarketplacePurchaseHistory",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RenewalAt",
                table: "MarketplacePurchaseHistory",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory",
                column: "EntitlementPurchaseId",
                filter: "\"EntitlementPurchaseId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingId",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingId",
                filter: "\"MarketplaceBookingId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingSubscriptionId",
                filter: "\"MarketplaceBookingSubscriptionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId_EventType_Oc~",
                table: "MarketplacePurchaseHistory",
                columns: new[] { "SourceType", "SourceId", "EventType", "OccurredAt" },
                filter: "\"EventType\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId_IdempotencyK~",
                table: "MarketplacePurchaseHistory",
                columns: new[] { "SourceType", "SourceId", "IdempotencyKey" },
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId_OccurredAt_R~",
                table: "MarketplacePurchaseHistory",
                columns: new[] { "SourceType", "SourceId", "OccurredAt", "RecordedAt", "Id" },
                filter: "\"EventType\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId_EventType_Oc~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId_IdempotencyK~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId_OccurredAt_R~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "CancellationEffectiveAt",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "CancellationRequestedAt",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventAmount",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventAutoRenew",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventCancelAtPeriodEnd",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventCreditQuantity",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventCurrency",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventEntitlementStatus",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventIsDeleted",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventReason",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventRemainingCreditQuantity",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventSubscriptionStatus",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "OccurredAt",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "PreviousPaymentStatus",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "PreviousRefundStatus",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropColumn(
                name: "RenewalAt",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_EntitlementPurchaseId",
                table: "MarketplacePurchaseHistory",
                column: "EntitlementPurchaseId",
                unique: true,
                filter: "\"EntitlementPurchaseId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingId",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingId",
                unique: true,
                filter: "\"MarketplaceBookingId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingSubscriptionId",
                unique: true,
                filter: "\"MarketplaceBookingSubscriptionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId",
                table: "MarketplacePurchaseHistory",
                columns: new[] { "SourceType", "SourceId" },
                unique: true);
        }
    }
}
