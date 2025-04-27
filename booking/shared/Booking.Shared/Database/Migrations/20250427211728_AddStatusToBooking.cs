using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "CONFIRMED",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "WORKING_FROM_OFFICE");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Booking",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Status",
                table: "Booking",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_Status",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "WORKING_FROM_OFFICE",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "CONFIRMED");
        }
    }
}
