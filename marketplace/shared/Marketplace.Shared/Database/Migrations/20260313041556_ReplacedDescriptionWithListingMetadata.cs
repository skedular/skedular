using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedDescriptionWithListingMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductVersion");

            migrationBuilder.AddColumn<ListingMetadata>(
                name: "ListingMetadata",
                table: "ProductVersion",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListingMetadata",
                table: "ProductVersion");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductVersion",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);
        }
    }
}
