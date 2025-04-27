using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeOrganizationNullableOnLocationAndTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Organization_OrganizationId",
                table: "Team");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "Team",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Organization_OrganizationId",
                table: "Team",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Organization_OrganizationId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Organization_OrganizationId",
                table: "Team");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "Team",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Organization_OrganizationId",
                table: "Team",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
