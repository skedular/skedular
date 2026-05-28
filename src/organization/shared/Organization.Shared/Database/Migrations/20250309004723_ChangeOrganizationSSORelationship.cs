using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrganizationSSORelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationSsoSetting_OrganizationId",
                table: "OrganizationSsoSetting");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationSsoSetting",
                type: "character varying(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationSsoSetting_OrganizationId",
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

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSsoSetting_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationSsoSetting_Organization_OrganizationId",
                table: "OrganizationSsoSetting",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
