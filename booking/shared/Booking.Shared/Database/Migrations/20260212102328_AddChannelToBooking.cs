using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "PRIVATE");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Channel",
                table: "Booking",
                column: "Channel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_Channel",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Booking");
        }
    }
}
