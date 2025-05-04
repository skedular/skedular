using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAMountAndCurrencyToStripeCheckoutSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountTotal",
                table: "StripeCheckoutSession",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "StripeCheckoutSession",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountTotal",
                table: "StripeCheckoutSession");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "StripeCheckoutSession");
        }
    }
}
