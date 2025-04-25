using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerStripePaymenthodMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "StripePaymentMethod",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePaymentMethod_CustomerId",
                table: "StripePaymentMethod",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripePaymentMethod_Customer_CustomerId",
                table: "StripePaymentMethod",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripePaymentMethod_Customer_CustomerId",
                table: "StripePaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_StripePaymentMethod_CustomerId",
                table: "StripePaymentMethod");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "StripePaymentMethod");
        }
    }
}
