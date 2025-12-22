using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDefaultIndetityTypeValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Identity",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "REGISTERED");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Identity",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "REGISTERED",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
