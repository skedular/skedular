using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationStripeConnectAccountToStripeConnectAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                table: "OrganizationStripeConnectAccount",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationStripeConnectAccount",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                table: "OrganizationStripeConnectAccount",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
