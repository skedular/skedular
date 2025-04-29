using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationStripeConnectAccountToStripeConnectAccountPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_OrganizationStripeConnectAccount_OrganizationStripe~",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationStripeConnectAccount",
                table: "OrganizationStripeConnectAccount");

            migrationBuilder.RenameTable(
                name: "OrganizationStripeConnectAccount",
                newName: "StripeConnectAccount");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStripeConnectAccount_OrganizationId",
                table: "StripeConnectAccount",
                newName: "IX_StripeConnectAccount_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStripeConnectAccount_ModifiedAt",
                table: "StripeConnectAccount",
                newName: "IX_StripeConnectAccount_ModifiedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStripeConnectAccount_DeletedAt",
                table: "StripeConnectAccount",
                newName: "IX_StripeConnectAccount_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStripeConnectAccount_CreatedAt",
                table: "StripeConnectAccount",
                newName: "IX_StripeConnectAccount_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StripeConnectAccount",
                table: "StripeConnectAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_StripeConnectAccount_OrganizationStripeConnectAccou~",
                table: "Product",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeConnectAccount_Organization_OrganizationId",
                table: "StripeConnectAccount",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_StripeConnectAccount_OrganizationStripeConnectAccou~",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_StripeConnectAccount_Organization_OrganizationId",
                table: "StripeConnectAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StripeConnectAccount",
                table: "StripeConnectAccount");

            migrationBuilder.RenameTable(
                name: "StripeConnectAccount",
                newName: "OrganizationStripeConnectAccount");

            migrationBuilder.RenameIndex(
                name: "IX_StripeConnectAccount_OrganizationId",
                table: "OrganizationStripeConnectAccount",
                newName: "IX_OrganizationStripeConnectAccount_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_StripeConnectAccount_ModifiedAt",
                table: "OrganizationStripeConnectAccount",
                newName: "IX_OrganizationStripeConnectAccount_ModifiedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StripeConnectAccount_DeletedAt",
                table: "OrganizationStripeConnectAccount",
                newName: "IX_OrganizationStripeConnectAccount_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StripeConnectAccount_CreatedAt",
                table: "OrganizationStripeConnectAccount",
                newName: "IX_OrganizationStripeConnectAccount_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationStripeConnectAccount",
                table: "OrganizationStripeConnectAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStripeConnectAccount_Organization_OrganizationId",
                table: "OrganizationStripeConnectAccount",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_OrganizationStripeConnectAccount_OrganizationStripe~",
                table: "Product",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "OrganizationStripeConnectAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "OrganizationStripeConnectAccount",
                principalColumn: "Id");
        }
    }
}
