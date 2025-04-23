using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationAuthorizedToStripeConnectAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplicationAuthorized",
                table: "OrganizationStripeConnectAccount",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStripeConnectAccount_ApplicationAuthorized",
                table: "OrganizationStripeConnectAccount",
                column: "ApplicationAuthorized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationStripeConnectAccount_ApplicationAuthorized",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropColumn(
                name: "ApplicationAuthorized",
                table: "OrganizationStripeConnectAccount");
        }
    }
}
