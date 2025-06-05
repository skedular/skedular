using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCdnFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CdnFile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdnFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CdnFileProduct",
                columns: table => new
                {
                    CdnFilesId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdnFileProduct", x => new { x.CdnFilesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_CdnFileProduct_CdnFile_CdnFilesId",
                        column: x => x.CdnFilesId,
                        principalTable: "CdnFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CdnFileProduct_Product_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CdnFileProductVersion",
                columns: table => new
                {
                    CdnFilesId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdnFileProductVersion", x => new { x.CdnFilesId, x.ProductVersionsId });
                    table.ForeignKey(
                        name: "FK_CdnFileProductVersion_CdnFile_CdnFilesId",
                        column: x => x.CdnFilesId,
                        principalTable: "CdnFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CdnFileProductVersion_ProductVersion_ProductVersionsId",
                        column: x => x.ProductVersionsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CdnFile_CreatedAt",
                table: "CdnFile",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CdnFile_ModifiedAt",
                table: "CdnFile",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CdnFileProduct_ProductsId",
                table: "CdnFileProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_CdnFileProductVersion_ProductVersionsId",
                table: "CdnFileProductVersion",
                column: "ProductVersionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CdnFileProduct");

            migrationBuilder.DropTable(
                name: "CdnFileProductVersion");

            migrationBuilder.DropTable(
                name: "CdnFile");
        }
    }
}
