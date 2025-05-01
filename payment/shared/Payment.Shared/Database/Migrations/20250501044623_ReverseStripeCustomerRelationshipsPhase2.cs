using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReverseStripeCustomerRelationshipsPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_StripeCustomer_StripeCustomerId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_StripeCustomer_StripeCustomerId",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Customer_StripeCustomerId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_CustomerId",
                table: "StripeCustomer",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCustomer_OrganizationId",
                table: "StripeCustomer",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeCustomer_Customer_CustomerId",
                table: "StripeCustomer",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeCustomer_Organization_OrganizationId",
                table: "StripeCustomer",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeCustomer_Customer_CustomerId",
                table: "StripeCustomer");

            migrationBuilder.DropForeignKey(
                name: "FK_StripeCustomer_Organization_OrganizationId",
                table: "StripeCustomer");

            migrationBuilder.DropIndex(
                name: "IX_StripeCustomer_CustomerId",
                table: "StripeCustomer");

            migrationBuilder.DropIndex(
                name: "IX_StripeCustomer_OrganizationId",
                table: "StripeCustomer");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Organization",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Customer",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_StripeCustomerId",
                table: "Organization",
                column: "StripeCustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_StripeCustomerId",
                table: "Customer",
                column: "StripeCustomerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_StripeCustomer_StripeCustomerId",
                table: "Customer",
                column: "StripeCustomerId",
                principalTable: "StripeCustomer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_StripeCustomer_StripeCustomerId",
                table: "Organization",
                column: "StripeCustomerId",
                principalTable: "StripeCustomer",
                principalColumn: "Id");
        }
    }
}
