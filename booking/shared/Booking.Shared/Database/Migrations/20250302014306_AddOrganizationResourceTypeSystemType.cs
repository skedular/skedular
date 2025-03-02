using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationResourceTypeSystemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "OrganizationResourceType",
                newName: "SystemType");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationResourceType_Type",
                table: "OrganizationResourceType",
                newName: "IX_OrganizationResourceType_SystemType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrganizationResourceType",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SystemType",
                table: "OrganizationResourceType",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationResourceType_SystemType",
                table: "OrganizationResourceType",
                newName: "IX_OrganizationResourceType_Type");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrganizationResourceType",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);
        }
    }
}
