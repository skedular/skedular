using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTaggedDesk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationTagId",
                table: "Customer",
                type: "character varying(100)",
                nullable: true);

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
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    TaggedDesksId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskOrganizationTag", x => new { x.OrganizationTagsId, x.TaggedDesksId });
                    table.ForeignKey(
                        name: "FK_DeskOrganizationTag_Desk_TaggedDesksId",
                        column: x => x.TaggedDesksId,
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
                name: "IX_Customer_OrganizationTagId",
                table: "Customer",
                column: "OrganizationTagId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskOrganizationTag_TaggedDesksId",
                table: "DeskOrganizationTag",
                column: "TaggedDesksId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_OrganizationTag_OrganizationTagId",
                table: "Customer",
                column: "OrganizationTagId",
                principalTable: "OrganizationTag",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_OrganizationTag_OrganizationTagId",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "DeskOrganizationTag");

            migrationBuilder.DropTable(
                name: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_Customer_OrganizationTagId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "OrganizationTagId",
                table: "Customer");
        }
    }
}
