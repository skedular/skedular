using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToSsoSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrganizationSsoSetting",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrganizationSsoSetting");
        }
    }
}
