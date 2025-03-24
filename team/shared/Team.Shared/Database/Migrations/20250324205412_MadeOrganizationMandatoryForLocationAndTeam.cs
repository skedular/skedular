using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeOrganizationMandatoryForLocationAndTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "Location",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "Location",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }
    }
}
