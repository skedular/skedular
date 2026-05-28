using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxAllowedResourceLockTimeToProductAndProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidByCard",
                table: "ProductVersion",
                type: "integer",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidThroughBankAccount",
                table: "ProductVersion",
                type: "integer",
                nullable: false,
                defaultValue: 43200);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidByCard",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidThroughBankAccount",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 43200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidByCard",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidThroughBankAccount",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidByCard",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidThroughBankAccount",
                table: "Product");
        }
    }
}
