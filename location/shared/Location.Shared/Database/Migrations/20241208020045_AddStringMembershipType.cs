using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStringMembershipType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewStatus",
                table: "JoinInvitation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "LocationMember",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                computedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "NewMembershipType",
                table: "JoinInvitation",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                computedColumnSql: "\n                    CASE \n                        WHEN \"MembershipType\" = 0 THEN 'OWNER'\n                        WHEN \"MembershipType\" = 1 THEN 'ADMINISTRATOR'\n                        WHEN \"MembershipType\" = 2 THEN 'MEMBER'\n                        ELSE 'UNKNOWN'\n                    END",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "LocationMember");

            migrationBuilder.DropColumn(
                name: "NewMembershipType",
                table: "JoinInvitation");

            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "JoinInvitation");
        }
    }
}
