using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalStripeConnectToStripeCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeConnectAccountId",
                table: "StripeCustomer",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_StripeConnectAccountId",
                table: "StripeCustomer",
                column: "StripeConnectAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeCustomer_StripeConnectAccount_StripeConnectAccountId",
                table: "StripeCustomer",
                column: "StripeConnectAccountId",
                principalTable: "StripeConnectAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeCustomer_StripeConnectAccount_StripeConnectAccountId",
                table: "StripeCustomer");

            migrationBuilder.DropIndex(
                name: "IX_StripeCustomer_StripeConnectAccountId",
                table: "StripeCustomer");

            migrationBuilder.DropColumn(
                name: "StripeConnectAccountId",
                table: "StripeCustomer");
        }
    }
}
