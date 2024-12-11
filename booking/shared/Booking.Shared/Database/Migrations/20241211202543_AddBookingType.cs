using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "WORKING_FROM_OFFICE");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Type",
                table: "Booking",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_Type",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Booking");
        }
    }
}
