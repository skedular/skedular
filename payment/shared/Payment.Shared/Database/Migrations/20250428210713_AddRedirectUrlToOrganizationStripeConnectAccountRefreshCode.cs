using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRedirectUrlToOrganizationStripeConnectAccountRefreshCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RedirectUrl",
                table: "OrganizationStripeConnectAccountRefreshCode",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedirectUrl",
                table: "OrganizationStripeConnectAccountRefreshCode");
        }
    }
}
