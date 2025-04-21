using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanynameEmailPhoneToStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Email",
                table: "OrganizationStripeConnectAccount",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Phone",
                table: "OrganizationStripeConnectAccount",
                column: "Phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_Email",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_Phone",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "OrganizationStripeConnectAccount");
        }
    }
}
