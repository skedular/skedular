using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class PreparingStripeCustomerMigrationPhase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_StripeCustomerIdTemp",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "StripeCustomerIdTemp",
                table: "Organization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerIdTemp",
                table: "Organization",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_StripeCustomerIdTemp",
                table: "Organization",
                column: "StripeCustomerIdTemp");
        }
    }
}
