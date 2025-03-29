using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Organization",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "PRIVATE");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Type",
                table: "Organization",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_Type",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Organization");
        }
    }
}
