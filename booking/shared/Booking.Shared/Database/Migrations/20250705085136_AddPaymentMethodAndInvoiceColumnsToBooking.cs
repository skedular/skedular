using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodAndInvoiceColumnsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "PAYMENT_CONFIRMED",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "CONFIRMED");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                table: "Booking",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendInvoice",
                table: "Booking",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PaymentMethod",
                table: "Booking",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_SendInvoice",
                table: "Booking",
                column: "SendInvoice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_PaymentMethod",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_SendInvoice",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "SendInvoice",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Booking",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "CONFIRMED",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "PAYMENT_CONFIRMED");
        }
    }
}
