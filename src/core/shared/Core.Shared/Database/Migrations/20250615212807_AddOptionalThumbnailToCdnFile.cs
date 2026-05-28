using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalThumbnailToCdnFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailCdnUrl",
                table: "CdnFile",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailContentType",
                table: "CdnFile",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailHeight",
                table: "CdnFile",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailStorageUrl",
                table: "CdnFile",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailWidth",
                table: "CdnFile",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailCdnUrl",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "ThumbnailContentType",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "ThumbnailHeight",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "ThumbnailStorageUrl",
                table: "CdnFile");

            migrationBuilder.DropColumn(
                name: "ThumbnailWidth",
                table: "CdnFile");
        }
    }
}
