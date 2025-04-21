using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalFieldToStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CapabilitiesCardPayments",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CapabilitiesTransfers",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingUrl",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_CapabilitiesCardPayments",
                table: "OrganizationStripeConnectAccount",
                column: "CapabilitiesCardPayments");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_CapabilitiesTransfers",
                table: "OrganizationStripeConnectAccount",
                column: "CapabilitiesTransfers");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Country",
                table: "OrganizationStripeConnectAccount",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount",
                column: "OnboardingCompletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_CapabilitiesCardPayments",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_CapabilitiesTransfers",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_Country",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "CapabilitiesCardPayments",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "CapabilitiesTransfers",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "OnboardingUrl",
                table: "OrganizationStripeConnectAccount");
        }
    }
}
