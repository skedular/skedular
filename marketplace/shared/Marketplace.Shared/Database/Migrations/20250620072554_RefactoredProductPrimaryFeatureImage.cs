using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredProductPrimaryFeatureImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImageUrl",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImageUrl",
                table: "Product");

            migrationBuilder.AddColumn<CdnImageFile>(
                name: "PrimaryFeatureImage",
                table: "ProductVersion",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<CdnImageFile>(
                name: "PrimaryFeatureImage",
                table: "Product",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImage",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImage",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryFeatureImageUrl",
                table: "ProductVersion",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryFeatureImageUrl",
                table: "Product",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
