using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalContentTypeWidthAndHeightToCdnFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "CdnFile",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "CdnFile",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "CdnFile",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CdnFile_ContentType",
                table: "CdnFile",
                column: "ContentType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CdnFile_ContentType",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "CdnFile");
        }
    }
}
