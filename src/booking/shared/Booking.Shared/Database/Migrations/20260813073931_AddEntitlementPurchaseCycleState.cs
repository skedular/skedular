using System;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitlementPurchaseCycleState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoRenew",
                table: "Entitlement",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CancelAtPeriodEnd",
                table: "Entitlement",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextRenewalAt",
                table: "Entitlement",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenewalFailureReason",
                table: "Entitlement",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntitlementPurchase",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PaymentConfirmedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProductPricing = table.Column<ProductPricing>(type: "jsonb", nullable: false),
                    CheckoutReturnUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentInstructions = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    StripeCheckoutSessionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StripeCheckoutUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StripePaymentIntentId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StripeAccountId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    InvoiceEmailList = table.Column<string>(type: "jsonb", nullable: false),
                    RenewalOfPurchaseId = table.Column<string>(type: "character varying(100)", nullable: true),
                    RenewalReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    EntitlementId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitlementPurchase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitlementPurchase_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitlementPurchase_EntitlementPurchase_RenewalOfPurchaseId",
                        column: x => x.RenewalOfPurchaseId,
                        principalTable: "EntitlementPurchase",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EntitlementPurchase_Entitlement_EntitlementId",
                        column: x => x.EntitlementId,
                        principalTable: "Entitlement",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EntitlementPurchase_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitlementPurchase_ProductVersion_ProductVersionId",
                        column: x => x.ProductVersionId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_NextRenewalAt",
                table: "Entitlement",
                column: "NextRenewalAt");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_CreatedAt",
                table: "EntitlementPurchase",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_CustomerId",
                table: "EntitlementPurchase",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_EntitlementId",
                table: "EntitlementPurchase",
                column: "EntitlementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_ModifiedAt",
                table: "EntitlementPurchase",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_OrganizationId",
                table: "EntitlementPurchase",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_PaymentExpiry",
                table: "EntitlementPurchase",
                column: "PaymentExpiry");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_PaymentStatus",
                table: "EntitlementPurchase",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_ProductVersionId",
                table: "EntitlementPurchase",
                column: "ProductVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_RenewalOfPurchaseId",
                table: "EntitlementPurchase",
                column: "RenewalOfPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_RenewalReference",
                table: "EntitlementPurchase",
                column: "RenewalReference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntitlementPurchase");

            migrationBuilder.DropIndex(
                name: "IX_Entitlement_NextRenewalAt",
                table: "Entitlement");

            migrationBuilder.DropColumn(
                name: "AutoRenew",
                table: "Entitlement");

            migrationBuilder.DropColumn(
                name: "CancelAtPeriodEnd",
                table: "Entitlement");

            migrationBuilder.DropColumn(
                name: "NextRenewalAt",
                table: "Entitlement");

            migrationBuilder.DropColumn(
                name: "RenewalFailureReason",
                table: "Entitlement");
        }
    }
}
