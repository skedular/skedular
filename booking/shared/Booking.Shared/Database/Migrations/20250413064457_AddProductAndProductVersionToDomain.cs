using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndProductVersionToDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Organization",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "PRIVATE");

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                    Price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PriceUnit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    PricePerMinute = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MinDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    MaxDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    BookAllLocationResources = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RecurrenceWindowDays = table.Column<int>(type: "integer", nullable: false),
                    RequireConsecutiveDays = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MaxBookingSpreadDays = table.Column<int>(type: "integer", nullable: true),
                    NumberOfResourcesToBook = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
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
                    ProductVersionProductTagId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProductVersion", x => new { x.ProductTagsId, x.ProductVersionProductTagId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion_OrganizationTag_ProductTagsId",
                        column: x => x.ProductTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion_ProductVersion_ProductVersion~",
                        column: x => x.ProductVersionProductTagId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagProductVersion1",
                columns: table => new
                {
                    LocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionLocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProductVersion1", x => new { x.LocationTagsId, x.ProductVersionLocationTagsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                        column: x => x.LocationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion1_ProductVersion_ProductVersio~",
                        column: x => x.ProductVersionLocationTagsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Type",
                table: "Organization",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProductVersion_ProductVersionProductTagId",
                table: "OrganizationTagProductVersion",
                column: "ProductVersionProductTagId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProductVersion1_ProductVersionLocationTagsId",
                table: "OrganizationTagProductVersion1",
                column: "ProductVersionLocationTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CreatedAt",
                table: "Product",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_DeletedAt",
                table: "Product",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ModifiedAt",
                table: "Product",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrganizationId",
                table: "Product",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_CreatedAt",
                table: "ProductVersion",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Currency",
                table: "ProductVersion",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_ModifiedAt",
                table: "ProductVersion",
                column: "ModifiedAt");

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

            migrationBuilder.DropIndex(
                name: "IX_Organization_Type",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Organization");
        }
    }
}
