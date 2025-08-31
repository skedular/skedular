using Location.Shared.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedLocationContactDetailsWithExtraMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Location");

            migrationBuilder.AddColumn<LocationExtraMetadata>(
                name: "ExtraMetadata",
                table: "Location",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraMetadata",
                table: "Location");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Location",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Location",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
