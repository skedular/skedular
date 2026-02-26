using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToBookingRecurringBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "RecurringBooking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_Category",
                table: "RecurringBooking",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_Category",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "RecurringBooking");
        }
    }
}
