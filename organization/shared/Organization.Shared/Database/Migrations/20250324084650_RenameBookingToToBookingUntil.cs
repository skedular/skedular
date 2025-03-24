using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameBookingToToBookingUntil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "To",
                table: "Booking",
                newName: "Until");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_To",
                table: "Booking",
                newName: "IX_Booking_Until");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Until",
                table: "Booking",
                newName: "To");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_Until",
                table: "Booking",
                newName: "IX_Booking_To");
        }
    }
}
