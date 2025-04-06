using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeOrganizationMandatoryOnSsoSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationSsoSetting",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationSsoSetting",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }
    }
}
