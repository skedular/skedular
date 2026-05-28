using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerFacingTermsAndConditionsUrlToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerFacingTermsAndConditionsUrl",
                table: "Organization",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Organization",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Organization",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Organization",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Website",
                table: "Organization",
                column: "Website");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_Name",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_Website",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "CustomerFacingTermsAndConditionsUrl",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Organization");
        }
    }
}
