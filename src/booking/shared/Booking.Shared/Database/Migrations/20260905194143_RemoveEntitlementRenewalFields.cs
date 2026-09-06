using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEntitlementRenewalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntitlementPurchase_EntitlementPurchase_RenewalOfPurchaseId",
                table: "EntitlementPurchase");

            migrationBuilder.DropIndex(
                name: "IX_EntitlementPurchase_RenewalOfPurchaseId",
                table: "EntitlementPurchase");

            migrationBuilder.DropIndex(
                name: "IX_EntitlementPurchase_RenewalReference",
                table: "EntitlementPurchase");

            migrationBuilder.DropIndex(
                name: "IX_Entitlement_NextRenewalAt",
                table: "Entitlement");

            migrationBuilder.DropColumn(
                name: "AutoRenew",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "RenewalOfPurchaseId",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "RenewalReference",
                table: "EntitlementPurchase");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoRenew",
                table: "EntitlementPurchase",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RenewalOfPurchaseId",
                table: "EntitlementPurchase",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenewalReference",
                table: "EntitlementPurchase",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_RenewalOfPurchaseId",
                table: "EntitlementPurchase",
                column: "RenewalOfPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_RenewalReference",
                table: "EntitlementPurchase",
                column: "RenewalReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entitlement_NextRenewalAt",
                table: "Entitlement",
                column: "NextRenewalAt");

            migrationBuilder.AddForeignKey(
                name: "FK_EntitlementPurchase_EntitlementPurchase_RenewalOfPurchaseId",
                table: "EntitlementPurchase",
                column: "RenewalOfPurchaseId",
                principalTable: "EntitlementPurchase",
                principalColumn: "Id");
        }
    }
}
