using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRatePercentage",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountExcludeTax",
                table: "Booking",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TaxAmount",
                table: "Booking",
                column: "TaxAmount");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TaxRatePercentage",
                table: "Booking",
                column: "TaxRatePercentage");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TotalAmountExcludeTax",
                table: "Booking",
                column: "TotalAmountExcludeTax");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_TaxAmount",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TaxRatePercentage",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TotalAmountExcludeTax",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TaxRatePercentage",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TotalAmountExcludeTax",
                table: "Booking");
        }
    }
}
