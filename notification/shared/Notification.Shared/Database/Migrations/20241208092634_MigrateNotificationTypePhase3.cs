using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MigrateNotificationTypePhase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewType",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notification",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notification",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "NewType",
                table: "Notification",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }
    }
}
