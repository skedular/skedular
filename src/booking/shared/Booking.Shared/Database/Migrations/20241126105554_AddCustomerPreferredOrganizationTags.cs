using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPreferredOrganizationTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_OrganizationTag_OrganizationTagId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_OrganizationTagId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "OrganizationTagId",
                table: "Customer");

            migrationBuilder.CreateTable(
                name: "CustomerOrganizationTag",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredOrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrganizationTag", x => new { x.PreferredByCustomersId, x.PreferredOrganizationTagsId });
                    table.ForeignKey(
                        name: "FK_CustomerOrganizationTag_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOrganizationTag_OrganizationTag_PreferredOrganizati~",
                        column: x => x.PreferredOrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrganizationTag_PreferredOrganizationTagsId",
                table: "CustomerOrganizationTag",
                column: "PreferredOrganizationTagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrganizationTag");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationTagId",
                table: "Customer",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_OrganizationTagId",
                table: "Customer",
                column: "OrganizationTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_OrganizationTag_OrganizationTagId",
                table: "Customer",
                column: "OrganizationTagId",
                principalTable: "OrganizationTag",
                principalColumn: "Id");
        }
    }
}
