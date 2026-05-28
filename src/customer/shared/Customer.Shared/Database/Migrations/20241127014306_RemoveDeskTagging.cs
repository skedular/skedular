using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeskTagging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeskLocationTag");

            migrationBuilder.DropTable(
                name: "DeskOrganizationTag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeskLocationTag",
                columns: table => new
                {
                    TaggedDesksId = table.Column<string>(type: "character varying(100)", nullable: false),
                    TagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeskLocationTag", x => new { x.TaggedDesksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_DeskLocationTag_Desk_TaggedDesksId",
                        column: x => x.TaggedDesksId,
                        principalTable: "Desk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeskLocationTag_LocationTag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "LocationTag",
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
                name: "IX_DeskLocationTag_TagsId",
                table: "DeskLocationTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_DeskOrganizationTag_TaggedDesksId",
                table: "DeskOrganizationTag",
                column: "TaggedDesksId");
        }
    }
}
