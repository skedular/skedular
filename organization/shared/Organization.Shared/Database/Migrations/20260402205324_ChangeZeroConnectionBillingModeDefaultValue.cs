using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeZeroConnectionBillingModeDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BillingMode",
                table: "OrganizationXeroConnection",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DISABLED",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Disabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BillingMode",
                table: "OrganizationXeroConnection",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Disabled",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "DISABLED");
        }
    }
}
