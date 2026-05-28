using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationTagFromProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTagProductVersion1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationTagProductVersion1",
                columns: table => new
                {
                    LocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductVersionLocationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagProductVersion1", x => new { x.LocationTagsId, x.ProductVersionLocationTagsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion1_OrganizationTag_LocationTags~",
                        column: x => x.LocationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagProductVersion1_ProductVersion_ProductVersio~",
                        column: x => x.ProductVersionLocationTagsId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagProductVersion1_ProductVersionLocationTagsId",
                table: "OrganizationTagProductVersion1",
                column: "ProductVersionLocationTagsId");
        }
    }
}
