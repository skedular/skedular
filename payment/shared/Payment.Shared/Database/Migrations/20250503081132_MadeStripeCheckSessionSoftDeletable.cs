using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeStripeCheckSessionSoftDeletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "StripeCheckoutSession",
                newName: "CheckoutUrl");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "StripeCheckoutSession",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "StripeCheckoutSession",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_DeletedAt",
                table: "StripeCheckoutSession",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripeCheckoutSession_DeletedAt",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StripeCheckoutSession");

            migrationBuilder.RenameColumn(
                name: "CheckoutUrl",
                table: "StripeCheckoutSession",
                newName: "Url");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "StripeCheckoutSession",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
