using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddInactiveToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Inactive",
                table: "Product",
                type: "boolean",
                nullable: true,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Product_Inactive",
                table: "Product",
                column: "Inactive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_Inactive",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Inactive",
                table: "Product");
        }
    }
}
