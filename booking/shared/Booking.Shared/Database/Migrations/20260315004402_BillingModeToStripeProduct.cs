using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class BillingModeToStripeProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingMode",
                table: "StripeProduct",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_BillingMode",
                table: "StripeProduct",
                column: "BillingMode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_BillingMode",
                table: "StripeProduct");

            migrationBuilder.DropColumn(
                name: "BillingMode",
                table: "StripeProduct");
        }
    }
}
