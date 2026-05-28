using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddHasRecurringInstanceOverridesToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasRecurringInstanceOverrides",
                table: "Booking",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_HasRecurringInstanceOverrides",
                table: "Booking",
                column: "HasRecurringInstanceOverrides");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_HasRecurringInstanceOverrides",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "HasRecurringInstanceOverrides",
                table: "Booking");
        }
    }
}
