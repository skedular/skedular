using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeConnectAccountId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_Name",
                table: "OrganizationStripeConnectAccount",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_StripeAccountId",
                table: "OrganizationStripeConnectAccount",
                column: "StripeAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_Name",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_StripeAccountId",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "OrganizationStripeConnectAccount");
        }
    }
}
