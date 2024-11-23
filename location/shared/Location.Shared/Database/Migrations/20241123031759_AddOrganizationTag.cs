using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationTag_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeskOrganizationTag",
                columns: table => new
                {
                    DesksId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskOrganizationTag", x => new { x.DesksId, x.OrganizationTagsId });
                    table.ForeignKey(
                        name: "FK_DeskOrganizationTag_Desk_DesksId",
                        column: x => x.DesksId,
                        principalTable: "Desk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeskOrganizationTag_OrganizationTag_OrganizationTagsId",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeskOrganizationTag_OrganizationTagsId",
                table: "DeskOrganizationTag",
                column: "OrganizationTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_DeletedAt",
                table: "OrganizationTag",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_Name",
                table: "OrganizationTag",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_OrganizationId",
                table: "OrganizationTag",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeskOrganizationTag");

            migrationBuilder.DropTable(
                name: "OrganizationTag");
        }
    }
}
