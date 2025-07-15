using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InvitedById = table.Column<string>(type: "character varying(100)", nullable: true),
                    InviteeId = table.Column<string>(type: "character varying(100)", nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    TeamId = table.Column<string>(type: "character varying(100)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SourceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Customer_InvitedById",
                        column: x => x.InvitedById,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notification_Customer_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notification_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notification_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notification_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_DeletedAt",
                table: "Notification",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_EventRaisedAt",
                table: "Notification",
                column: "EventRaisedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_InvitedById",
                table: "Notification",
                column: "InvitedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_InviteeId",
                table: "Notification",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_LocationId",
                table: "Notification",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ModifiedAt",
                table: "Notification",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OrganizationId",
                table: "Notification",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SourceId",
                table: "Notification",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TeamId",
                table: "Notification",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Type",
                table: "Notification",
                column: "Type");
        }
    }
}
