using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationToPrecomputedLocationProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "PrecomputedLocationProduct",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PrecomputedLocationProduct_OrganizationId",
                table: "PrecomputedLocationProduct",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecomputedLocationProduct_Organization_OrganizationId",
                table: "PrecomputedLocationProduct",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecomputedLocationProduct_Organization_OrganizationId",
                table: "PrecomputedLocationProduct");

            migrationBuilder.DropIndex(
                name: "IX_PrecomputedLocationProduct_OrganizationId",
                table: "PrecomputedLocationProduct");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "PrecomputedLocationProduct");
        }
    }
}
