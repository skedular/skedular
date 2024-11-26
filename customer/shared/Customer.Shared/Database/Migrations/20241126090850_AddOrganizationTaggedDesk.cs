using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTaggedDesk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_DeskOrganizationTag_TaggedDesksId",
                table: "DeskOrganizationTag",
                column: "TaggedDesksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeskOrganizationTag");
        }
    }
}
