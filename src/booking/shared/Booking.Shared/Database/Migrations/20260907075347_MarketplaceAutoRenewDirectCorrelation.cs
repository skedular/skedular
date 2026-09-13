using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MarketplaceAutoRenewDirectCorrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripePrice_StripeProductId",
                table: "StripePrice");

            migrationBuilder.AddColumn<string>(
                name: "BillingMode",
                table: "StripePrice",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "StripePrice",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "StripePrice",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxInclusive",
                table: "StripePrice",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MembershipTerm",
                table: "StripePrice",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductPricingId",
                table: "StripePrice",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "StripePrice",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitAmount",
                table: "StripePrice",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StripeCancelAtPeriodEnd",
                table: "MarketplaceBookingSubscription",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StripeCurrentPeriodEndsAt",
                table: "MarketplaceBookingSubscription",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionStatus",
                table: "MarketplaceBookingSubscription",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AutoRenew",
                table: "EntitlementPurchase",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StripeCancelAtPeriodEnd",
                table: "EntitlementPurchase",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StripeCurrentPeriodEndsAt",
                table: "EntitlementPurchase",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "EntitlementPurchase",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "EntitlementPurchase",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "EntitlementPurchase",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionStatus",
                table: "EntitlementPurchase",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_StripeAccountId_ProductPricingId_UnitAmount_Cur~",
                table: "StripePrice",
                columns: new[] { "StripeAccountId", "ProductPricingId", "UnitAmount", "Currency", "MembershipTerm", "BillingMode", "IsTaxInclusive", "IsRecurring" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_StripeAccountId_StripePriceId",
                table: "StripePrice",
                columns: new[] { "StripeAccountId", "StripePriceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_StripeProductId",
                table: "StripePrice",
                column: "StripeProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_StripeAccountId_StripeSubscr~",
                table: "MarketplaceBookingSubscription",
                columns: new[] { "StripeAccountId", "StripeSubscriptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntitlementPurchase_StripeAccountId_StripeSubscriptionId",
                table: "EntitlementPurchase",
                columns: new[] { "StripeAccountId", "StripeSubscriptionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripePrice_StripeAccountId_ProductPricingId_UnitAmount_Cur~",
                table: "StripePrice");

            migrationBuilder.DropIndex(
                name: "IX_StripePrice_StripeAccountId_StripePriceId",
                table: "StripePrice");

            migrationBuilder.DropIndex(
                name: "IX_StripePrice_StripeProductId",
                table: "StripePrice");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingSubscription_StripeAccountId_StripeSubscr~",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropIndex(
                name: "IX_EntitlementPurchase_StripeAccountId_StripeSubscriptionId",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "BillingMode",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "IsTaxInclusive",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "MembershipTerm",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "ProductPricingId",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "UnitAmount",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "StripeCancelAtPeriodEnd",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "StripeCurrentPeriodEndsAt",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionStatus",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "AutoRenew",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "StripeCancelAtPeriodEnd",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "StripeCurrentPeriodEndsAt",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "EntitlementPurchase");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionStatus",
                table: "EntitlementPurchase");

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_StripeProductId",
                table: "StripePrice",
                column: "StripeProductId",
                unique: true);
        }
    }
}
