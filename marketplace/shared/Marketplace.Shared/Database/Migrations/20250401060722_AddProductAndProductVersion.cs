using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVersion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PriceUnit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    PricePerMinute = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MinDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    MaxDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    BookAllLocationResources = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RecurrenceIntervalDays = table.Column<int>(type: "integer", nullable: false),
                    ForceContinuousSlots = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MaxSpreadDays = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVersion_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagProductVersion",
                columns: table => new
                {
                    ProductTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductTagsId1 = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProductVersion", x => new { x.ProductTagsId, x.ProductTagsId1 });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId1",
                        column: x => x.ProductTagsId1,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion_ProductVersion_ProductTagsId",
                        column: x => x.ProductTagsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagProductVersion1",
                columns: table => new
                {
                    LocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    LocationTagsId1 = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProductVersion1", x => new { x.LocationTagsId, x.LocationTagsId1 });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                        column: x => x.LocationTagsId1,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion1_ProductVersion_LocationTagsId",
                        column: x => x.LocationTagsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProductVersion_ProductTagsId1",
                table: "OrganizationTagProductVersion",
                column: "ProductTagsId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProductVersion1_LocationTagsId1",
                table: "OrganizationTagProductVersion1",
                column: "LocationTagsId1");

            migrationBuilder.CreateIndex(
                name: "IX_Product_DeletedAt",
                table: "Product",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrganizationId",
                table: "Product",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Currency",
                table: "ProductVersion",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Description",
                table: "ProductVersion",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Name",
                table: "ProductVersion",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_PricePerMinute",
                table: "ProductVersion",
                column: "PricePerMinute");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_ProductId",
                table: "ProductVersion",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTagProductVersion");

            migrationBuilder.DropTable(
                name: "OrganizationTagProductVersion1");

            migrationBuilder.DropTable(
                name: "ProductVersion");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
