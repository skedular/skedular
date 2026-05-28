using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeBookingCheckoutSessionReplicated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingCheckoutSession_PaymentReferenceId",
                table: "BookingCheckoutSession");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "BookingCheckoutSession",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentReferenceId",
                table: "BookingCheckoutSession",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CheckoutUrl",
                table: "BookingCheckoutSession",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "BookingCheckoutSession",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EventRaisedAt",
                table: "BookingCheckoutSession",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_DeletedAt",
                table: "BookingCheckoutSession",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingCheckoutSession_DeletedAt",
                table: "BookingCheckoutSession");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BookingCheckoutSession");

            migrationBuilder.DropColumn(
                name: "EventRaisedAt",
                table: "BookingCheckoutSession");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "BookingCheckoutSession",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentReferenceId",
                table: "BookingCheckoutSession",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckoutUrl",
                table: "BookingCheckoutSession",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_PaymentReferenceId",
                table: "BookingCheckoutSession",
                column: "PaymentReferenceId");
        }
    }
}
