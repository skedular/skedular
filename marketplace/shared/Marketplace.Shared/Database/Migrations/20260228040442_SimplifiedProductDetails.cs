using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTagProduct");

            migrationBuilder.DropTable(
                name: "OrganizationTagProduct1");

            migrationBuilder.DropIndex(
                name: "IX_Product_Currency",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_IsPriceTaxInclusive",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Name",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_PricePerMinute",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "AcceptedBookingPaymentMethods",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "BookAllLocationResources",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "FeatureImages",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsPriceTaxInclusive",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidViaBankTransfer",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MaxAllowedResourcesLockTimePaidViaCard",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MaxDurationMinutes",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MinDurationMinutes",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "NumberOfResourcesToBook",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PricePerMinute",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationTagId",
                table: "Product",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationTagId1",
                table: "Product",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrganizationTagId",
                table: "Product",
                column: "OrganizationTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrganizationTagId1",
                table: "Product",
                column: "OrganizationTagId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_OrganizationTag_OrganizationTagId",
                table: "Product",
                column: "OrganizationTagId",
                principalTable: "OrganizationTag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_OrganizationTag_OrganizationTagId1",
                table: "Product",
                column: "OrganizationTagId1",
                principalTable: "OrganizationTag",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_OrganizationTag_OrganizationTagId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_OrganizationTag_OrganizationTagId1",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_OrganizationTagId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_OrganizationTagId1",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "OrganizationTagId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "OrganizationTagId1",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "AcceptedBookingPaymentMethods",
                table: "Product",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "BookAllLocationResources",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Product",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Product",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.AddColumn<ICollection<CdnImageFile>>(
                name: "FeatureImages",
                table: "Product",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceTaxInclusive",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidViaBankTransfer",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 43200);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedResourcesLockTimePaidViaCard",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "MaxDurationMinutes",
                table: "Product",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinDurationMinutes",
                table: "Product",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Product",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfResourcesToBook",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Product",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerMinute",
                table: "Product",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceUnit",
                table: "Product",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrganizationTagProduct",
                columns: table => new
                {
                    ProductProductTagId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProduct", x => new { x.ProductProductTagId, x.ProductTagsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProduct_OrganizationTag_ProductTagsId",
                        column: x => x.ProductTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProduct_Product_ProductProductTagId",
                        column: x => x.ProductProductTagId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagProduct1",
                columns: table => new
                {
                    LocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductLocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProduct1", x => new { x.LocationTagsId, x.ProductLocationTagsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProduct1_OrganizationTag_LocationTagsId",
                        column: x => x.LocationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProduct1_Product_ProductLocationTagsId",
                        column: x => x.ProductLocationTagsId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_Currency",
                table: "Product",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IsPriceTaxInclusive",
                table: "Product",
                column: "IsPriceTaxInclusive");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Name",
                table: "Product",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Product_PricePerMinute",
                table: "Product",
                column: "PricePerMinute");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProduct_ProductTagsId",
                table: "OrganizationTagProduct",
                column: "ProductTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProduct1_ProductLocationTagsId",
                table: "OrganizationTagProduct1",
                column: "ProductLocationTagsId");
        }
    }
}
