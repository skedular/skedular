using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPriceTaxInclusiveToProductAndProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPriceTaxInclusive",
                table: "ProductVersion",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceTaxInclusive",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_IsPriceTaxInclusive",
                table: "ProductVersion",
                column: "IsPriceTaxInclusive");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IsPriceTaxInclusive",
                table: "Product",
                column: "IsPriceTaxInclusive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_IsPriceTaxInclusive",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_Product_IsPriceTaxInclusive",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsPriceTaxInclusive",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "IsPriceTaxInclusive",
                table: "Product");
        }
    }
}
