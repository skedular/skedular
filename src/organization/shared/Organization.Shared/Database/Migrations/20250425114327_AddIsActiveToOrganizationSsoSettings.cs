using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToOrganizationSsoSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrganizationSsoSetting",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_IsActive",
                table: "OrganizationSsoSetting",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationSsoSetting_IsActive",
                table: "OrganizationSsoSetting");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrganizationSsoSetting");
        }
    }
}
