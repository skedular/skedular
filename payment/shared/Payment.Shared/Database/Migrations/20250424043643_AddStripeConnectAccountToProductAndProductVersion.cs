using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeConnectAccountToProductAndProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion",
                column: "OrganizationStripeConnectAccountId",
                principalTable: "OrganizationStripeConnectAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_OrganizationStripeConnectAccount_Organizatio~",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_OrganizationStripeConnectAccountId",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "OrganizationStripeConnectAccountId",
                table: "ProductVersion");
        }
    }
}
