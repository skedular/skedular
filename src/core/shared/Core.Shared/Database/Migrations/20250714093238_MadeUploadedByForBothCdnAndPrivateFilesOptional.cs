using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeUploadedByForBothCdnAndPrivateFilesOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CdnFile_Customer_UploadedById",
                table: "CdnFile");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateFile_Customer_UploadedById",
                table: "PrivateFile");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedById",
                table: "PrivateFile",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedById",
                table: "CdnFile",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_CdnFile_Customer_UploadedById",
                table: "CdnFile",
                column: "UploadedById",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateFile_Customer_UploadedById",
                table: "PrivateFile",
                column: "UploadedById",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CdnFile_Customer_UploadedById",
                table: "CdnFile");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateFile_Customer_UploadedById",
                table: "PrivateFile");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedById",
                table: "PrivateFile",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UploadedById",
                table: "CdnFile",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CdnFile_Customer_UploadedById",
                table: "CdnFile",
                column: "UploadedById",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateFile_Customer_UploadedById",
                table: "PrivateFile",
                column: "UploadedById",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
