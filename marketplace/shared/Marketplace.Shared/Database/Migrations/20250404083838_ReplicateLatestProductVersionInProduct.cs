using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplicateLatestProductVersionInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId1",
                table: "OrganizationTagProductVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion_ProductVersion_ProductTagsId",
                table: "OrganizationTagProductVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                table: "OrganizationTagProductVersion1");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion1_ProductVersion_LocationTagsId",
                table: "OrganizationTagProductVersion1");

            migrationBuilder.RenameColumn(
                name: "LocationTagsId1",
                table: "OrganizationTagProductVersion1",
                newName: "ProductVersionLocationTagsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTagProductVersion1_LocationTagsId1",
                table: "OrganizationTagProductVersion1",
                newName: "IX_OrganizationTagProductVersion1_ProductVersionLocationTagsId");

            migrationBuilder.RenameColumn(
                name: "ProductTagsId1",
                table: "OrganizationTagProductVersion",
                newName: "ProductVersionProductTagId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTagProductVersion_ProductTagsId1",
                table: "OrganizationTagProductVersion",
                newName: "IX_OrganizationTagProductVersion_ProductVersionProductTagId");

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

            migrationBuilder.AddColumn<bool>(
                name: "ForceContinuousSlots",
                table: "Product",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxDurationMinutes",
                table: "Product",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxSpreadDays",
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

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceIntervalDays",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId",
                table: "OrganizationTagProductVersion",
                column: "ProductTagsId",
                principalTable: "OrganizationTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion_ProductVersion_ProductVersion~",
                table: "OrganizationTagProductVersion",
                column: "ProductVersionProductTagId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                table: "OrganizationTagProductVersion1",
                column: "LocationTagsId",
                principalTable: "OrganizationTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion1_ProductVersion_ProductVersio~",
                table: "OrganizationTagProductVersion1",
                column: "ProductVersionLocationTagsId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId",
                table: "OrganizationTagProductVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion_ProductVersion_ProductVersion~",
                table: "OrganizationTagProductVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                table: "OrganizationTagProductVersion1");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTagProductVersion1_ProductVersion_ProductVersio~",
                table: "OrganizationTagProductVersion1");

            migrationBuilder.DropTable(
                name: "OrganizationTagProduct");

            migrationBuilder.DropTable(
                name: "OrganizationTagProduct1");

            migrationBuilder.DropIndex(
                name: "IX_Product_Currency",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Name",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_PricePerMinute",
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
                name: "ForceContinuousSlots",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MaxDurationMinutes",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MaxSpreadDays",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MinDurationMinutes",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Name",
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

            migrationBuilder.DropColumn(
                name: "RecurrenceIntervalDays",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ProductVersionLocationTagsId",
                table: "OrganizationTagProductVersion1",
                newName: "LocationTagsId1");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTagProductVersion1_ProductVersionLocationTagsId",
                table: "OrganizationTagProductVersion1",
                newName: "IX_OrganizationTagProductVersion1_LocationTagsId1");

            migrationBuilder.RenameColumn(
                name: "ProductVersionProductTagId",
                table: "OrganizationTagProductVersion",
                newName: "ProductTagsId1");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTagProductVersion_ProductVersionProductTagId",
                table: "OrganizationTagProductVersion",
                newName: "IX_OrganizationTagProductVersion_ProductTagsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId1",
                table: "OrganizationTagProductVersion",
                column: "ProductTagsId1",
                principalTable: "OrganizationTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion_ProductVersion_ProductTagsId",
                table: "OrganizationTagProductVersion",
                column: "ProductTagsId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                table: "OrganizationTagProductVersion1",
                column: "LocationTagsId1",
                principalTable: "OrganizationTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTagProductVersion1_ProductVersion_LocationTagsId",
                table: "OrganizationTagProductVersion1",
                column: "LocationTagsId",
                principalTable: "ProductVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
