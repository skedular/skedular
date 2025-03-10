using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAndResourceOpeningHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<OpeningHours>(
                name: "OpeningHours",
                table: "Resource",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OverrideOpeningHoursOverriden",
                table: "Resource",
                type: "boolean",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<OpeningHours>(
                name: "OpeningHours",
                table: "Location",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resource_OverrideOpeningHoursOverriden",
                table: "Resource",
                column: "OverrideOpeningHoursOverriden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resource_OverrideOpeningHoursOverriden",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "OverrideOpeningHoursOverriden",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "Location");
        }
    }
}
