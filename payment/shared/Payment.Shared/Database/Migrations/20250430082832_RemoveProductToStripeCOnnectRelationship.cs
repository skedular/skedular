using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductToStripeCOnnectRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_OrganizationStripeConnectAccountId",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "OrganizationStripeConnectAccountId",
                table: "ProductVersion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationStripeConnectAccountId",
                table: "ProductVersion",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_OrganizationStripeConnectAccountId",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_StripeConnectAccount_OrganizationStripeConne~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");
        }
    }
}
