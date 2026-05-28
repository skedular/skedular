using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedProductColumnsFromOrganizationTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
