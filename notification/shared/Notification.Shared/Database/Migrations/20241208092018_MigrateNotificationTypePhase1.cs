using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MigrateNotificationTypePhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewType",
                table: "Notification",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
            
            migrationBuilder.Sql(@"UPDATE public.""Notification""
                                    SET ""NewType""= CASE 
                                        WHEN ""Type"" = 0 THEN 'INVITATION_TO_JOIN_ORGANIZATION'
                                        WHEN ""Type"" = 1 THEN 'INVITATION_TO_JOIN_LOCATION'
                                        WHEN ""Type"" = 2 THEN 'INVITATION_TO_JOIN_TEAM'
                                        ELSE 'UNKNOWN'
                                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewType",
                table: "Notification");
        }
    }
}
