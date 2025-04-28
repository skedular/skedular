using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class DropStripeCheckoutSessionFromBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_StripeCheckoutSessionId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "StripeCheckoutSessionId",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_StripeCheckoutSessionId",
                table: "Booking",
                column: "StripeCheckoutSessionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking",
                column: "StripeCheckoutSessionId",
                principalTable: "StripeCheckoutSession",
                principalColumn: "Id");
        }
    }
}
