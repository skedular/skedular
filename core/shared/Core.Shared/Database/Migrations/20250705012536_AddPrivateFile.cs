using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivateFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivateFile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StorageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailStorageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ThumbnailContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ThumbnailWidth = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailHeight = table.Column<int>(type: "integer", nullable: true),
                    UploadedById = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateFile_Customer_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateFile_ContentType",
                table: "PrivateFile",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateFile_CreatedAt",
                table: "PrivateFile",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateFile_ModifiedAt",
                table: "PrivateFile",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateFile_UploadedById",
                table: "PrivateFile",
                column: "UploadedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateFile");
        }
    }
}
