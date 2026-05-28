using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinInvitation");

            migrationBuilder.DropTable(
                name: "LocationMember");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoinInvitation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(100)", nullable: false),
                    InviteeId = table.Column<string>(type: "character varying(100)", nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinInvitation_Customer_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinInvitation_Customer_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JoinInvitation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationMember_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_CreatedAt",
                table: "JoinInvitation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_CreatedById",
                table: "JoinInvitation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_DeletedAt",
                table: "JoinInvitation",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Email",
                table: "JoinInvitation",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_InviteeId",
                table: "JoinInvitation",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_LocationId",
                table: "JoinInvitation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_ModifiedAt",
                table: "JoinInvitation",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Role",
                table: "JoinInvitation",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Status",
                table: "JoinInvitation",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CreatedAt",
                table: "LocationMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId_LocationId",
                table: "LocationMember",
                columns: new[] { "CustomerId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_DeletedAt",
                table: "LocationMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_LocationId",
                table: "LocationMember",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_ModifiedAt",
                table: "LocationMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_Role",
                table: "LocationMember",
                column: "Role");
        }
    }
}
