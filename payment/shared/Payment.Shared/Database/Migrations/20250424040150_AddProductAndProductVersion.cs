using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
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
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PriceUnit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PricePerMinute = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
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
                name: "ProductVersion");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
