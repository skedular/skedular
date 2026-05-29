using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTaxDetailsIsRegistered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "OrganizationTaxDetails",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTaxDetails_IsRegistered",
                table: "OrganizationTaxDetails",
                column: "IsRegistered");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationTaxDetails_IsRegistered",
                table: "OrganizationTaxDetails");

            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "OrganizationTaxDetails");
        }
    }
}
