using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovePricePerMinute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_PricePerMinute",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "PricePerMinute",
                table: "ProductVersion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerMinute",
                table: "ProductVersion",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_PricePerMinute",
                table: "ProductVersion",
                column: "PricePerMinute");
        }
    }
}
