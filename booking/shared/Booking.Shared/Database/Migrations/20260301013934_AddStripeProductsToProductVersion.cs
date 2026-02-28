using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeProductsToProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeProduct_ProductVersion_ProductVersionId",
                table: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_ProductVersionId",
                table: "StripeProduct");

            migrationBuilder.AlterColumn<string>(
                name: "ProductVersionId",
                table: "StripeProduct",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PricingCadence",
                table: "StripeProduct",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_PricingCadence",
                table: "StripeProduct",
                column: "PricingCadence");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_ProductVersionId",
                table: "StripeProduct",
                column: "ProductVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_StripeProductId",
                table: "StripeProduct",
                column: "StripeProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeProduct_ProductVersion_ProductVersionId",
                table: "StripeProduct",
                column: "ProductVersionId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeProduct_ProductVersion_ProductVersionId",
                table: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_PricingCadence",
                table: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_ProductVersionId",
                table: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_StripeProductId",
                table: "StripeProduct");

            migrationBuilder.DropColumn(
                name: "PricingCadence",
                table: "StripeProduct");

            migrationBuilder.AlterColumn<string>(
                name: "ProductVersionId",
                table: "StripeProduct",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_ProductVersionId",
                table: "StripeProduct",
                column: "ProductVersionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StripeProduct_ProductVersion_ProductVersionId",
                table: "StripeProduct",
                column: "ProductVersionId",
                principalTable: "ProductVersion",
                principalColumn: "Id");
        }
    }
}
