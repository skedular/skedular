using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForCreatedAtAndUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_CreatedAt",
                table: "ProductVersion",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_ModifiedAt",
                table: "ProductVersion",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CreatedAt",
                table: "Product",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ModifiedAt",
                table: "Product",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_CreatedAt",
                table: "OrganizationTag",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_ModifiedAt",
                table: "OrganizationTag",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_CreatedAt",
                table: "OrganizationMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_ModifiedAt",
                table: "OrganizationMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_CreatedAt",
                table: "Organization",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ModifiedAt",
                table: "Organization",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_CreatedAt",
                table: "Identity",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_ModifiedAt",
                table: "Identity",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CreatedAt",
                table: "Customer",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ModifiedAt",
                table: "Customer",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_CreatedAt",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_ModifiedAt",
                table: "ProductVersion");

            migrationBuilder.DropIndex(
                name: "IX_Product_CreatedAt",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ModifiedAt",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_CreatedAt",
                table: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_ModifiedAt",
                table: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_CreatedAt",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_ModifiedAt",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_Organization_CreatedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Organization_ModifiedAt",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Identity_CreatedAt",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Identity_ModifiedAt",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CreatedAt",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_ModifiedAt",
                table: "Customer");
        }
    }
}
