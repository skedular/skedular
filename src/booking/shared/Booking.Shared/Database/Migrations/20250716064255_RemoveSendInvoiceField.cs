using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSendInvoiceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_SendInvoice",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "SendInvoice",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendInvoice",
                table: "Booking",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_SendInvoice",
                table: "Booking",
                column: "SendInvoice");
        }
    }
}
