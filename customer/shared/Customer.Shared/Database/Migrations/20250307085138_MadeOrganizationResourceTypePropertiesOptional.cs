using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeOrganizationResourceTypePropertiesOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationResourceType_Organization_OrganizationId",
                table: "OrganizationResourceType");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationResourceType",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationResourceType_Organization_OrganizationId",
                table: "OrganizationResourceType",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationResourceType_Organization_OrganizationId",
                table: "OrganizationResourceType");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationResourceType",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationResourceType_Organization_OrganizationId",
                table: "OrganizationResourceType",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
