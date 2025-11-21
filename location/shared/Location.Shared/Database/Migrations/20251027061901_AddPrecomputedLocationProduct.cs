using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecomputedLocationProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrecomputedLocationProduct",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecomputedLocationProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrecomputedLocationProduct_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecomputedLocationProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagPrecomputedLocationProduct",
                columns: table => new
                {
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PrecomputedLocationProductsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagPrecomputedLocationProduct", x => new { x.OrganizationTagsId, x.PrecomputedLocationProductsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagPrecomputedLocationProduct_OrganizationTag_O~",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagPrecomputedLocationProduct_PrecomputedLocati~",
                        column: x => x.PrecomputedLocationProductsId,
                        principalTable: "PrecomputedLocationProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagPrecomputedLocationProduct_PrecomputedLocati~",
                table: "OrganizationTagPrecomputedLocationProduct",
                column: "PrecomputedLocationProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecomputedLocationProduct_CreatedAt",
                table: "PrecomputedLocationProduct",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PrecomputedLocationProduct_LocationId",
                table: "PrecomputedLocationProduct",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecomputedLocationProduct_ModifiedAt",
                table: "PrecomputedLocationProduct",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PrecomputedLocationProduct_ProductId",
                table: "PrecomputedLocationProduct",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTagPrecomputedLocationProduct");

            migrationBuilder.DropTable(
                name: "PrecomputedLocationProduct");
        }
    }
}
