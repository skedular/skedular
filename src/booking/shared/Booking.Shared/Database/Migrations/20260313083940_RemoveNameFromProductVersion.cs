using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNameFromProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_Name",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "Name",
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
                name: "Name",
                table: "ProductVersion",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Name",
                table: "ProductVersion",
                column: "Name");
        }
    }
}
