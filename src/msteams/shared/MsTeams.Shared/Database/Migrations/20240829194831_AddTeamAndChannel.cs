using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsTeams.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AzureTenantTeam",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WebUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AzureTenantId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AzureTenantTeam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AzureTenantTeam_AzureTenant_AzureTenantId",
                        column: x => x.AzureTenantId,
                        principalTable: "AzureTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AzureTenantTeamChannel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WebUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    AzureTenantTeamId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AzureTenantTeamChannel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AzureTenantTeamChannel_AzureTenantTeam_AzureTenantTeamId",
                        column: x => x.AzureTenantTeamId,
                        principalTable: "AzureTenantTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeam_AzureTenantId",
                table: "AzureTenantTeam",
                column: "AzureTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeam_DeletedAt",
                table: "AzureTenantTeam",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeam_Name",
                table: "AzureTenantTeam",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeamChannel_AzureTenantTeamId",
                table: "AzureTenantTeamChannel",
                column: "AzureTenantTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeamChannel_DeletedAt",
                table: "AzureTenantTeamChannel",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureTenantTeamChannel_Name",
                table: "AzureTenantTeamChannel",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AzureTenantTeamChannel");

            migrationBuilder.DropTable(
                name: "AzureTenantTeam");
        }
    }
}
