using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCanonicalRefundReliability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_OrganizationId_LocalEntityType_LocalEntit~",
                table: "MarketplaceRefund");

            migrationBuilder.AddColumn<string>(
                name: "ChargeId",
                table: "StripeCheckoutSession",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChargeType",
                table: "StripeCheckoutSession",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationAccountId",
                table: "StripeCheckoutSession",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "StripeCheckoutSession",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PayoutDisbursedAt",
                table: "StripeCheckoutSession",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutFailureMessage",
                table: "StripeCheckoutSession",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutId",
                table: "StripeCheckoutSession",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutStatus",
                table: "StripeCheckoutSession",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "StripeCheckoutSession",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferId",
                table: "StripeCheckoutSession",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "MarketplaceRefundEvent",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewStatus",
                table: "MarketplaceRefundEvent",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousStatus",
                table: "MarketplaceRefundEvent",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ApprovedAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByCustomerId",
                table: "MarketplaceRefund",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankTransferReference",
                table: "MarketplaceRefund",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BankTransferSentAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalculationResultJson",
                table: "MarketplaceRefund",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "MarketplaceRefund",
                type: "character varying(100000)",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "MarketplaceRefund",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastNotificationStatus",
                table: "MarketplaceRefund",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastReconciledAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicySnapshotJson",
                table: "MarketplaceRefund",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PostPayoutRefund",
                table: "MarketplaceRefund",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReconciledAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReconciliationLeaseExpiresAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReconciliationLeaseOwner",
                table: "MarketplaceRefund",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReconciliationLeaseRenewedAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReconciliationStatus",
                table: "MarketplaceRefund",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundKind",
                table: "MarketplaceRefund",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Cancellation");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RejectedAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByCustomerId",
                table: "MarketplaceRefund",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "MarketplaceRefund",
                type: "character varying(100000)",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "MarketplaceRefund",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "MarketplaceRefund",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeChargeId",
                table: "MarketplaceRefund",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeChargeType",
                table: "MarketplaceRefund",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "MarketplaceRefund",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeRefundPath",
                table: "MarketplaceRefund",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StripeRefundPathSelectedAt",
                table: "MarketplaceRefund",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeTransferId",
                table: "MarketplaceRefund",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimezoneId",
                table: "MarketplaceRefund",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AllocatedRefundAmount",
                table: "MarketplaceBookingFailure",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedOccurrenceIds",
                table: "MarketplaceBookingFailure",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResolutionDeadlineAt",
                table: "MarketplaceBookingFailure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResolutionDecidedAt",
                table: "MarketplaceBookingFailure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionDecision",
                table: "MarketplaceBookingFailure",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnavailableOccurrenceIds",
                table: "MarketplaceBookingFailure",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Location",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MarketplaceExternalRefundReconciliation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StripeAccountId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalRefundId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Open"),
                    FirstSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    NextRetryAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolutionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ResolutionActorCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ResolutionCorrelationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceExternalRefundReconciliation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceRefundNotificationDelivery",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarketplaceRefundId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RecipientId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Pending"),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceRefundNotificationDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceRefundNotificationDelivery_MarketplaceRefund_Mar~",
                        column: x => x.MarketplaceRefundId,
                        principalTable: "MarketplaceRefund",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceRefundPaymentAllocation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourcePaymentProvider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SourcePaymentReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceCapturedAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    AllocatedRefundAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsSourcePayment = table.Column<bool>(type: "boolean", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MarketplaceRefundId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceRefundPaymentAllocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceRefundPaymentAllocation_MarketplaceRefund_Market~",
                        column: x => x.MarketplaceRefundId,
                        principalTable: "MarketplaceRefund",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_ActiveCancellation",
                table: "MarketplaceRefund",
                columns: new[] { "LocalEntityType", "LocalEntityId", "RefundKind" },
                unique: true,
                filter: "\"RefundKind\" = 'Cancellation' AND \"Status\" NOT IN ('Completed', 'Failed', 'Rejected', 'Cancelled')");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_ApprovedByCustomerId",
                table: "MarketplaceRefund",
                column: "ApprovedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_IdempotencyKey",
                table: "MarketplaceRefund",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_OrganizationId_LocalEntityType_LocalEntit~",
                table: "MarketplaceRefund",
                columns: new[] { "OrganizationId", "LocalEntityType", "LocalEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_ReconciliationLeaseExpiresAt_Status",
                table: "MarketplaceRefund",
                columns: new[] { "ReconciliationLeaseExpiresAt", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_RejectedByCustomerId",
                table: "MarketplaceRefund",
                column: "RejectedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_ResolutionDecision_ResolutionDead~",
                table: "MarketplaceBookingFailure",
                columns: new[] { "ResolutionDecision", "ResolutionDeadlineAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_CreatedAt",
                table: "MarketplaceExternalRefundReconciliation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_ModifiedAt",
                table: "MarketplaceExternalRefundReconciliation",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_OrganizationId_Stat~",
                table: "MarketplaceExternalRefundReconciliation",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_Provider_ExternalRe~",
                table: "MarketplaceExternalRefundReconciliation",
                columns: new[] { "Provider", "ExternalRefundId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_Status",
                table: "MarketplaceExternalRefundReconciliation",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_Status_NextRetryAt_~",
                table: "MarketplaceExternalRefundReconciliation",
                columns: new[] { "Status", "NextRetryAt", "RetryCount" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundNotificationDelivery_CreatedAt",
                table: "MarketplaceRefundNotificationDelivery",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundNotificationDelivery_MarketplaceRefundId_E~",
                table: "MarketplaceRefundNotificationDelivery",
                columns: new[] { "MarketplaceRefundId", "EventType", "RecipientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundNotificationDelivery_ModifiedAt",
                table: "MarketplaceRefundNotificationDelivery",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundNotificationDelivery_Status",
                table: "MarketplaceRefundNotificationDelivery",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundPaymentAllocation_CreatedAt",
                table: "MarketplaceRefundPaymentAllocation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundPaymentAllocation_MarketplaceRefundId_Sour~",
                table: "MarketplaceRefundPaymentAllocation",
                columns: new[] { "MarketplaceRefundId", "SourcePaymentProvider", "SourcePaymentReference", "IsSourcePayment" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundPaymentAllocation_ModifiedAt",
                table: "MarketplaceRefundPaymentAllocation",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundPaymentAllocation_SourcePaymentProvider_So~",
                table: "MarketplaceRefundPaymentAllocation",
                columns: new[] { "SourcePaymentProvider", "SourcePaymentReference" },
                unique: true,
                filter: "\"IsSourcePayment\" = TRUE");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceRefund_Customer_ApprovedByCustomerId",
                table: "MarketplaceRefund",
                column: "ApprovedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceRefund_Customer_RejectedByCustomerId",
                table: "MarketplaceRefund",
                column: "RejectedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceRefund_Customer_ApprovedByCustomerId",
                table: "MarketplaceRefund");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceRefund_Customer_RejectedByCustomerId",
                table: "MarketplaceRefund");

            migrationBuilder.DropTable(
                name: "MarketplaceExternalRefundReconciliation");

            migrationBuilder.DropTable(
                name: "MarketplaceRefundNotificationDelivery");

            migrationBuilder.DropTable(
                name: "MarketplaceRefundPaymentAllocation");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_ActiveCancellation",
                table: "MarketplaceRefund");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_ApprovedByCustomerId",
                table: "MarketplaceRefund");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_IdempotencyKey",
                table: "MarketplaceRefund");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_OrganizationId_LocalEntityType_LocalEntit~",
                table: "MarketplaceRefund");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_ReconciliationLeaseExpiresAt_Status",
                table: "MarketplaceRefund");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceRefund_RejectedByCustomerId",
                table: "MarketplaceRefund");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingFailure_ResolutionDecision_ResolutionDead~",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "ChargeId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "ChargeType",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "DestinationAccountId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "PayoutDisbursedAt",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "PayoutFailureMessage",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "PayoutId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "PayoutStatus",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "MarketplaceRefundEvent");

            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "MarketplaceRefundEvent");

            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "MarketplaceRefundEvent");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ApprovedByCustomerId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "BankTransferReference",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "BankTransferSentAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "CalculationResultJson",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "LastNotificationStatus",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "LastReconciledAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "PolicySnapshotJson",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "PostPayoutRefund",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ReconciledAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ReconciliationLeaseExpiresAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ReconciliationLeaseOwner",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ReconciliationLeaseRenewedAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "ReconciliationStatus",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "RefundKind",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "RejectedByCustomerId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripeChargeId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripeChargeType",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripeRefundPath",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripeRefundPathSelectedAt",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "StripeTransferId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "TimezoneId",
                table: "MarketplaceRefund");

            migrationBuilder.DropColumn(
                name: "AllocatedRefundAmount",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "CreatedOccurrenceIds",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "ResolutionDeadlineAt",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "ResolutionDecidedAt",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "ResolutionDecision",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "UnavailableOccurrenceIds",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_OrganizationId_LocalEntityType_LocalEntit~",
                table: "MarketplaceRefund",
                columns: new[] { "OrganizationId", "LocalEntityType", "LocalEntityId" },
                unique: true);
        }
    }
}
