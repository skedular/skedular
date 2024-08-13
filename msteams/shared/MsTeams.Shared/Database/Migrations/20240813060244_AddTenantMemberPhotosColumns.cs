using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantMemberPhotosColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrincipalName",
                table: "TenantMember");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "TenantMember",
                newName: "FamilyName");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "TenantMember",
                newName: "Designation");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TenantMember",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl120",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl240",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl360",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl432",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl48",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl504",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl64",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl648",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl96",
                table: "TenantMember",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl120",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl240",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl360",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl432",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl48",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl504",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl64",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl648",
                table: "TenantMember");

            migrationBuilder.DropColumn(
                name: "PhotoUrl96",
                table: "TenantMember");

            migrationBuilder.RenameColumn(
                name: "FamilyName",
                table: "TenantMember",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "Designation",
                table: "TenantMember",
                newName: "JobTitle");

            migrationBuilder.AddColumn<string>(
                name: "PrincipalName",
                table: "TenantMember",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);
        }
    }
}
