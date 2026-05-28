using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProductType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProductVersion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "RESOURCE");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Type",
                table: "ProductVersion",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVersion_Type",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductVersion");
        }
    }
}
