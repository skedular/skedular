using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantStripePriceFromProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripePrice_ProductVersion_ProductVersionId",
                table: "StripePrice");

            migrationBuilder.DropIndex(
                name: "IX_StripePrice_ProductVersionId",
                table: "StripePrice");

            migrationBuilder.DropColumn(
                name: "ProductVersionId",
                table: "StripePrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductVersionId",
                table: "StripePrice",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePrice_ProductVersionId",
                table: "StripePrice",
                column: "ProductVersionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StripePrice_ProductVersion_ProductVersionId",
                table: "StripePrice",
                column: "ProductVersionId",
                principalTable: "ProductVersion",
                principalColumn: "Id");
        }
    }
}
