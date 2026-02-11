using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamedBookingTypeToBookingCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_Type",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Booking");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "WORKING_FROM_OFFICE");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Category",
                table: "Booking",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_Category",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Booking");

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
    }
}
