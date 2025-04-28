using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeStripeCheckoutSessionIdOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "Booking",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking",
                column: "StripeCheckoutSessionId",
                principalTable: "StripeCheckoutSession",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "Booking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking",
                column: "StripeCheckoutSessionId",
                principalTable: "StripeCheckoutSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
