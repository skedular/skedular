using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameGstNumberToTaxId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GstPercentage",
                table: "OrganizationTaxDetails",
                newName: "TaxRatePercentage");

            migrationBuilder.RenameColumn(
                name: "GstNumber",
                table: "OrganizationTaxDetails",
                newName: "TaxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaxRatePercentage",
                table: "OrganizationTaxDetails",
                newName: "GstPercentage");

            migrationBuilder.RenameColumn(
                name: "TaxId",
                table: "OrganizationTaxDetails",
                newName: "GstNumber");
        }
    }
}
