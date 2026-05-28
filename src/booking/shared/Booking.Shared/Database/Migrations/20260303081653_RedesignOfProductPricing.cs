using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RedesignOfProductPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceBookingProductVersion");

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

            migrationBuilder.RenameColumn(
                name: "LineItems",
                table: "MarketplaceBooking",
                newName: "ProductPricing");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfResourcesToBook",
                table: "StripeProduct",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductPricingId",
                table: "StripeProduct",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "ProductVersion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "MarketplaceBooking",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductVersionId",
                table: "MarketplaceBooking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "MarketplaceBooking",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_NumberOfResourcesToBook",
                table: "StripeProduct",
                column: "NumberOfResourcesToBook");

            migrationBuilder.CreateIndex(
                name: "IX_StripeProduct_ProductPricingId",
                table: "StripeProduct",
                column: "ProductPricingId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_ProductVersionId",
                table: "MarketplaceBooking",
                column: "ProductVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBooking_Quantity",
                table: "MarketplaceBooking",
                column: "Quantity");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBooking_ProductVersion_ProductVersionId",
                table: "MarketplaceBooking",
                column: "ProductVersionId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBooking_ProductVersion_ProductVersionId",
                table: "MarketplaceBooking");

            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_NumberOfResourcesToBook",
                table: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_StripeProduct_ProductPricingId",
                table: "StripeProduct");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBooking_ProductVersionId",
                table: "MarketplaceBooking");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBooking_Quantity",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "NumberOfResourcesToBook",
                table: "StripeProduct");

            migrationBuilder.DropColumn(
                name: "ProductPricingId",
                table: "StripeProduct");

            migrationBuilder.DropColumn(
                name: "ProductVersionId",
                table: "MarketplaceBooking");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "MarketplaceBooking");

            migrationBuilder.RenameColumn(
                name: "ProductPricing",
                table: "MarketplaceBooking",
                newName: "LineItems");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "ProductVersion",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BookAllLocationResources",
                table: "ProductVersion",
                type: "boolean",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceTaxInclusive",
                table: "ProductVersion",
                type: "boolean",
                nullable: true);

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
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductVersion",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerMinute",
                table: "ProductVersion",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceUnit",
                table: "ProductVersion",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "MarketplaceBooking",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingProductVersion",
                columns: table => new
                {
                    MarketplaceBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingProductVersion", x => new { x.MarketplaceBookingsId, x.ProductVersionsId });
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingProductVersion_MarketplaceBooking_Marketp~",
                        column: x => x.MarketplaceBookingsId,
                        principalTable: "MarketplaceBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingProductVersion_ProductVersion_ProductVers~",
                        column: x => x.ProductVersionsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_PricePerMinute",
                table: "ProductVersion",
                column: "PricePerMinute");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingProductVersion_ProductVersionsId",
                table: "MarketplaceBookingProductVersion",
                column: "ProductVersionsId");
        }
    }
}
