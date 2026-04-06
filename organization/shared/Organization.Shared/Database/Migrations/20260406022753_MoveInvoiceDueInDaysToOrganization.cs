using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoveInvoiceDueInDaysToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDueInDays",
                table: "OrganizationBillingDetails");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceDueInDays",
                table: "Organization",
                type: "integer",
                nullable: false,
                defaultValue: 7);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_InvoiceDueInDays",
                table: "Organization",
                column: "InvoiceDueInDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_InvoiceDueInDays",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "InvoiceDueInDays",
                table: "Organization");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceDueInDays",
                table: "OrganizationBillingDetails",
                type: "integer",
                nullable: false,
                defaultValue: 7);
        }
    }
}
