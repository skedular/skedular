using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceTypeSystemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceType_Type",
                table: "ResourceType");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ResourceType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ResourceType",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "SystemType",
                table: "ResourceType",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_SystemType",
                table: "ResourceType",
                column: "SystemType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceType_SystemType",
                table: "ResourceType");

            migrationBuilder.DropColumn(
                name: "SystemType",
                table: "ResourceType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ResourceType",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ResourceType",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceType_Type",
                table: "ResourceType",
                column: "Type");
        }
    }
}
