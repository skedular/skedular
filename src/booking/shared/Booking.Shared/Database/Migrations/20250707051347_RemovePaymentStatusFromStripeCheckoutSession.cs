using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaymentStatusFromStripeCheckoutSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripeCheckoutSession_PaymentStatus",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "StripeCheckoutSession");

            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb",
                oldDefaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "StripeCheckoutSession",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb",
                oldDefaultValue: new string[0]);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_PaymentStatus",
                table: "StripeCheckoutSession",
                column: "PaymentStatus");
        }
    }
}
