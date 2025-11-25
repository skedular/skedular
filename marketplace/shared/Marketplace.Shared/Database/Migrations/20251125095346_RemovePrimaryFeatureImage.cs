using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovePrimaryFeatureImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImage",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImage",
                table: "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
