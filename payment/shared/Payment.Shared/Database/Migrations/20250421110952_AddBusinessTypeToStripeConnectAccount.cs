using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessTypeToStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessType",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_BusinessType",
                table: "OrganizationStripeConnectAccount",
                column: "BusinessType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_DefaultCurrency",
                table: "OrganizationStripeConnectAccount",
                column: "DefaultCurrency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_BusinessType",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_DefaultCurrency",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "OrganizationStripeConnectAccount");
        }
    }
}
