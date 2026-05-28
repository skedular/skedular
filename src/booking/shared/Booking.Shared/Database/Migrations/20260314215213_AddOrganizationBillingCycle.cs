using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationBillingCycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingCycle",
                table: "Organization",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "MONTHLY");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_BillingCycle",
                table: "Organization",
                column: "BillingCycle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_BillingCycle",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "BillingCycle",
                table: "Organization");
        }
    }
}
