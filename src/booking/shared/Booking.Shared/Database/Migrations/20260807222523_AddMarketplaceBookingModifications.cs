using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBookingModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_CustomerId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_DeletedByCustomerId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBookingSubscription_M~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBooking_MarketplaceBo~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceRefund_LatestRefundId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_Organization_OrganizationId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_ProductVersion_ProductVersionId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceRefundId",
                table: "MarketplaceRefundNotificationDelivery",
                type: "character varying(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SourceId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ProductVersionId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceBookingId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LatestRefundId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedByCustomerId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResolutionActorCustomerId",
                table: "MarketplaceExternalRefundReconciliation",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "MarketplaceExternalRefundReconciliation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActorCustomerId",
                table: "MarketplaceBookingFailureEvent",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientCustomerId",
                table: "MarketplaceBookingFailureDelivery",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecurringBookingId",
                table: "MarketplaceBookingFailure",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceBookingSubscriptionId",
                table: "MarketplaceBookingFailure",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "MarketplaceBookingFailure",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingModification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActorKind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Reason = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResultFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResultUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalResourceIds = table.Column<string>(type: "jsonb", nullable: false),
                    ResultResourceIds = table.Column<string>(type: "jsonb", nullable: false),
                    SubscriptionOccurrenceOverride = table.Column<bool>(type: "boolean", nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ActorCustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingModification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingModification_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingModification_Customer_ActorCustomerId",
                        column: x => x.ActorCustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingModificationNotificationDelivery",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeliveryKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    LastAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    MarketplaceBookingModificationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RecipientCustomerId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingModificationNotificationDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingModificationNotificationDelivery_Customer~",
                        column: x => x.RecipientCustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingModificationNotificationDelivery_Marketpl~",
                        column: x => x.MarketplaceBookingModificationId,
                        principalTable: "MarketplaceBookingModification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_ResolutionActorCust~",
                table: "MarketplaceExternalRefundReconciliation",
                column: "ResolutionActorCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureEvent_ActorCustomerId",
                table: "MarketplaceBookingFailureEvent",
                column: "ActorCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureDelivery_RecipientCustomerId",
                table: "MarketplaceBookingFailureDelivery",
                column: "RecipientCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure",
                column: "ResolutionActorCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModification_ActorCustomerId",
                table: "MarketplaceBookingModification",
                column: "ActorCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModification_BookingId_OccurredAt",
                table: "MarketplaceBookingModification",
                columns: new[] { "BookingId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModification_CreatedAt",
                table: "MarketplaceBookingModification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModification_ModifiedAt",
                table: "MarketplaceBookingModification",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModificationNotificationDelivery_CreatedAt",
                table: "MarketplaceBookingModificationNotificationDelivery",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModificationNotificationDelivery_Marketpl~",
                table: "MarketplaceBookingModificationNotificationDelivery",
                columns: new[] { "MarketplaceBookingModificationId", "DeliveryKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModificationNotificationDelivery_Modified~",
                table: "MarketplaceBookingModificationNotificationDelivery",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModificationNotificationDelivery_Recipien~",
                table: "MarketplaceBookingModificationNotificationDelivery",
                column: "RecipientCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingModificationNotificationDelivery_Status_L~",
                table: "MarketplaceBookingModificationNotificationDelivery",
                columns: new[] { "Status", "LastAttemptAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingFailure_Booking_BookingId",
                table: "MarketplaceBookingFailure",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingFailure_Booking_RecurringBookingId",
                table: "MarketplaceBookingFailure",
                column: "RecurringBookingId",
                principalTable: "Booking",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingFailure_Customer_ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure",
                column: "ResolutionActorCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingFailure_MarketplaceBookingSubscription_Ma~",
                table: "MarketplaceBookingFailure",
                column: "MarketplaceBookingSubscriptionId",
                principalTable: "MarketplaceBookingSubscription",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingFailureDelivery_Customer_RecipientCustome~",
                table: "MarketplaceBookingFailureDelivery",
                column: "RecipientCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingFailureEvent_Customer_ActorCustomerId",
                table: "MarketplaceBookingFailureEvent",
                column: "ActorCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceExternalRefundReconciliation_Customer_Resolution~",
                table: "MarketplaceExternalRefundReconciliation",
                column: "ResolutionActorCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_CustomerId",
                table: "MarketplacePurchaseHistory",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_DeletedByCustomerId",
                table: "MarketplacePurchaseHistory",
                column: "DeletedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBookingSubscription_M~",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingSubscriptionId",
                principalTable: "MarketplaceBookingSubscription",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBooking_MarketplaceBo~",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingId",
                principalTable: "MarketplaceBooking",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceRefund_LatestRefundId",
                table: "MarketplacePurchaseHistory",
                column: "LatestRefundId",
                principalTable: "MarketplaceRefund",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_Organization_OrganizationId",
                table: "MarketplacePurchaseHistory",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_ProductVersion_ProductVersionId",
                table: "MarketplacePurchaseHistory",
                column: "ProductVersionId",
                principalTable: "ProductVersion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingFailure_Booking_BookingId",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingFailure_Booking_RecurringBookingId",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingFailure_Customer_ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingFailure_MarketplaceBookingSubscription_Ma~",
                table: "MarketplaceBookingFailure");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingFailureDelivery_Customer_RecipientCustome~",
                table: "MarketplaceBookingFailureDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingFailureEvent_Customer_ActorCustomerId",
                table: "MarketplaceBookingFailureEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceExternalRefundReconciliation_Customer_Resolution~",
                table: "MarketplaceExternalRefundReconciliation");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_CustomerId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_DeletedByCustomerId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBookingSubscription_M~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBooking_MarketplaceBo~",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceRefund_LatestRefundId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_Organization_OrganizationId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplacePurchaseHistory_ProductVersion_ProductVersionId",
                table: "MarketplacePurchaseHistory");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingModificationNotificationDelivery");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingModification");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceExternalRefundReconciliation_ResolutionActorCust~",
                table: "MarketplaceExternalRefundReconciliation");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingFailureEvent_ActorCustomerId",
                table: "MarketplaceBookingFailureEvent");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingFailureDelivery_RecipientCustomerId",
                table: "MarketplaceBookingFailureDelivery");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingFailure_ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure");

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceRefundId",
                table: "MarketplaceRefundNotificationDelivery",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AlterColumn<string>(
                name: "SourceId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ProductVersionId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceBookingId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LatestRefundId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedByCustomerId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "MarketplacePurchaseHistory",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResolutionActorCustomerId",
                table: "MarketplaceExternalRefundReconciliation",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "MarketplaceExternalRefundReconciliation",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActorCustomerId",
                table: "MarketplaceBookingFailureEvent",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientCustomerId",
                table: "MarketplaceBookingFailureDelivery",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResolutionActorCustomerId",
                table: "MarketplaceBookingFailure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecurringBookingId",
                table: "MarketplaceBookingFailure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceBookingSubscriptionId",
                table: "MarketplaceBookingFailure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "MarketplaceBookingFailure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_CustomerId",
                table: "MarketplacePurchaseHistory",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_Customer_DeletedByCustomerId",
                table: "MarketplacePurchaseHistory",
                column: "DeletedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBookingSubscription_M~",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingSubscriptionId",
                principalTable: "MarketplaceBookingSubscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceBooking_MarketplaceBo~",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingId",
                principalTable: "MarketplaceBooking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_MarketplaceRefund_LatestRefundId",
                table: "MarketplacePurchaseHistory",
                column: "LatestRefundId",
                principalTable: "MarketplaceRefund",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_Organization_OrganizationId",
                table: "MarketplacePurchaseHistory",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplacePurchaseHistory_ProductVersion_ProductVersionId",
                table: "MarketplacePurchaseHistory",
                column: "ProductVersionId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
