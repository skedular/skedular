using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RedesignOfProductPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_IsPriceTaxInclusive",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_PricePerMinute",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "BookAllLocationResources",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "IsPriceTaxInclusive",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidViaBankTransfer",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidViaCard",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "MaxDurationMinutes",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "MinDurationMinutes",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "NumberOfResourcesToBook",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "PricePerMinute",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "ProductVersion");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "ProductVersion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "ProductVersion",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "BookAllLocationResources",
                table: "ProductVersion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceTaxInclusive",
                table: "ProductVersion",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidViaBankTransfer",
                table: "ProductVersion",
                type: "integer",
                nullable: false,
                defaultValue: 43200);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidViaCard",
                table: "ProductVersion",
                type: "integer",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "MaxDurationMinutes",
                table: "ProductVersion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinDurationMinutes",
                table: "ProductVersion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfResourcesToBook",
                table: "ProductVersion",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductVersion",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerMinute",
                table: "ProductVersion",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceUnit",
                table: "ProductVersion",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_IsPriceTaxInclusive",
                table: "ProductVersion",
                column: "IsPriceTaxInclusive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_PricePerMinute",
                table: "ProductVersion",
                column: "PricePerMinute");
        }
    }
}
