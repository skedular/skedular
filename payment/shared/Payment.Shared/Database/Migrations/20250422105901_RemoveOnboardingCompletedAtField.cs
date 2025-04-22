using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOnboardingCompletedAtField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_OnboardingCompletedAt",
                table: "OrganizationStripeConnectAccount",
                column: "OnboardingCompletedAt");
        }
    }
}
