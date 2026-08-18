using System;
using Booking.Shared.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditBasedEntitlements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "MarketplaceExternalRefundReconciliation",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntitlementId",
                table: "MarketplaceBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsumingCreditLedgerEntryId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Entitlement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PurchaseReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PricingId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GrantedQuantity = table.Column<int>(type: "integer", nullable: false),
                    CreditUnit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ActivatesAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RefundUnusedCredits = table.Column<bool>(type: "boolean", nullable: false),
                    NetPurchaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entitlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entitlement_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entitlement_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditLedgerEntry",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ReferenceKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ActorOrSource = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Metadata = table.Column<CreditLedgerEntryMetadata>(type: "jsonb", nullable: true),
                    EntitlementId = table.Column<string>(type: "character varying(100)", nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditLedgerEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditLedgerEntry_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditLedgerEntry_Entitlement_EntitlementId",
                        column: x => x.EntitlementId,
                        principalTable: "Entitlement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitlementRefundLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnusedCreditQuantity = table.Column<int>(type: "integer", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    EntitlementId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MarketplaceRefundId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitlementRefundLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitlementRefundLink_Entitlement_EntitlementId",
                        column: x => x.EntitlementId,
                        principalTable: "Entitlement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitlementRefundLink_MarketplaceRefund_MarketplaceRefundId",
                        column: x => x.MarketplaceRefundId,
                        principalTable: "MarketplaceRefund",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_EntitlementId",
                table: "MarketplaceBooking",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ConsumingCreditLedgerEntryId",
                table: "Booking",
                column: "ConsumingCreditLedgerEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditLedgerEntry_BookingId",
                table: "CreditLedgerEntry",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditLedgerEntry_CreatedAt",
                table: "CreditLedgerEntry",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CreditLedgerEntry_EntitlementId_ReferenceKey",
                table: "CreditLedgerEntry",
                columns: new[] { "EntitlementId", "ReferenceKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditLedgerEntry_ModifiedAt",
                table: "CreditLedgerEntry",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_CreatedAt",
                table: "Entitlement",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_CustomerId_Status",
                table: "Entitlement",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_ExpiresAt",
                table: "Entitlement",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_ModifiedAt",
                table: "Entitlement",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_OrganizationId",
                table: "Entitlement",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_PurchaseReference",
                table: "Entitlement",
                column: "PurchaseReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementRefundLink_CreatedAt",
                table: "EntitlementRefundLink",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementRefundLink_EntitlementId",
                table: "EntitlementRefundLink",
                column: "EntitlementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementRefundLink_MarketplaceRefundId",
                table: "EntitlementRefundLink",
                column: "MarketplaceRefundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementRefundLink_ModifiedAt",
                table: "EntitlementRefundLink",
                column: "ModifiedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_CreditLedgerEntry_ConsumingCreditLedgerEntryId",
                table: "Booking",
                column: "ConsumingCreditLedgerEntryId",
                principalTable: "CreditLedgerEntry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_Entitlement_EntitlementId",
                table: "MarketplaceBooking",
                column: "EntitlementId",
                principalTable: "Entitlement",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceExternalRefundReconciliation_Organization_Organi~",
                table: "MarketplaceExternalRefundReconciliation",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_CreditLedgerEntry_ConsumingCreditLedgerEntryId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_Entitlement_EntitlementId",
                table: "MarketplaceBooking");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceExternalRefundReconciliation_Organization_Organi~",
                table: "MarketplaceExternalRefundReconciliation");

            migrationBuilder.DropTable(
                name: "CreditLedgerEntry");

            migrationBuilder.DropTable(
                name: "EntitlementRefundLink");

            migrationBuilder.DropTable(
                name: "Entitlement");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBooking_EntitlementId",
                table: "MarketplaceBooking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_ConsumingCreditLedgerEntryId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "EntitlementId",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "ConsumingCreditLedgerEntryId",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "MarketplaceExternalRefundReconciliation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);
        }
    }
}
