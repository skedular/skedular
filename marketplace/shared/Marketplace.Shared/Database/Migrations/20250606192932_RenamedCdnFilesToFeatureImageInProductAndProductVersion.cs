using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamedCdnFilesToFeatureImageInProductAndProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CdnFileProduct_CdnFile_CdnFilesId",
                table: "CdnFileProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CdnFileProductVersion_CdnFile_CdnFilesId",
                table: "CdnFileProductVersion");

            migrationBuilder.RenameColumn(
                name: "CdnFilesId",
                table: "CdnFileProductVersion",
                newName: "FeatureImagesId");

            migrationBuilder.RenameColumn(
                name: "CdnFilesId",
                table: "CdnFileProduct",
                newName: "FeatureImagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CdnFileProduct_CdnFile_FeatureImagesId",
                table: "CdnFileProduct",
                column: "FeatureImagesId",
                principalTable: "CdnFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CdnFileProductVersion_CdnFile_FeatureImagesId",
                table: "CdnFileProductVersion",
                column: "FeatureImagesId",
                principalTable: "CdnFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CdnFileProduct_CdnFile_FeatureImagesId",
                table: "CdnFileProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CdnFileProductVersion_CdnFile_FeatureImagesId",
                table: "CdnFileProductVersion");

            migrationBuilder.RenameColumn(
                name: "FeatureImagesId",
                table: "CdnFileProductVersion",
                newName: "CdnFilesId");

            migrationBuilder.RenameColumn(
                name: "FeatureImagesId",
                table: "CdnFileProduct",
                newName: "CdnFilesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CdnFileProduct_CdnFile_CdnFilesId",
                table: "CdnFileProduct",
                column: "CdnFilesId",
                principalTable: "CdnFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CdnFileProductVersion_CdnFile_CdnFilesId",
                table: "CdnFileProductVersion",
                column: "CdnFilesId",
                principalTable: "CdnFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
