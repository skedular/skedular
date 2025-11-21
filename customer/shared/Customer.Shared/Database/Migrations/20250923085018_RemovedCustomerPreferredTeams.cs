using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCustomerPreferredTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerTeam");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "Team");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerTeam",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredTeamsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTeam", x => new { x.PreferredByCustomersId, x.PreferredTeamsId });
                    table.ForeignKey(
                        name: "FK_CustomerTeam_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTeam_Team_PreferredTeamsId",
                        column: x => x.PreferredTeamsId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationMemberId = table.Column<string>(type: "character varying(100)", nullable: true),
                    TeamId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "ACTIVE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_OrganizationMember_OrganizationMemberId",
                        column: x => x.OrganizationMemberId,
                        principalTable: "OrganizationMember",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamMember_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTeam_PreferredTeamsId",
                table: "CustomerTeam",
                column: "PreferredTeamsId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_CreatedAt",
                table: "Team",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Team_DeletedAt",
                table: "Team",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ModifiedAt",
                table: "Team",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Team_OrganizationId",
                table: "Team",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CreatedAt",
                table: "TeamMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_CustomerId_TeamId",
                table: "TeamMember",
                columns: new[] { "CustomerId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_DeletedAt",
                table: "TeamMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_ModifiedAt",
                table: "TeamMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_OrganizationMemberId",
                table: "TeamMember",
                column: "OrganizationMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_Role",
                table: "TeamMember",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_Status",
                table: "TeamMember",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMember",
                column: "TeamId");
        }
    }
}
