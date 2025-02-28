using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceAndorganizationResourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationResourceType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationResourceType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationResourceType_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    RequireBookingApproval = table.Column<bool>(type: "boolean", nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    OrganizationResourceTypeId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resource_OrganizationResourceType_OrganizationResourceTypeId",
                        column: x => x.OrganizationResourceTypeId,
                        principalTable: "OrganizationResourceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingResource",
                columns: table => new
                {
                    BookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingResource", x => new { x.BookingsId, x.ResourcesId });
                    table.ForeignKey(
                        name: "FK_BookingResource_Booking_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingResource_Resource_ResourcesId",
                        column: x => x.ResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagResource",
                columns: table => new
                {
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagResource", x => new { x.OrganizationTagsId, x.ResourcesId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagResource_OrganizationTag_OrganizationTagsId",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagResource_Resource_ResourcesId",
                        column: x => x.ResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Room_Deactivated",
                table: "Room",
                column: "Deactivated");

            migrationBuilder.CreateIndex(
                name: "IX_Room_Name",
                table: "Room",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RequireBookingApproval",
                table: "Room",
                column: "RequireBookingApproval");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTag_Type",
                table: "OrganizationTag",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Location_About",
                table: "Location",
                column: "About");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DailyDeskCountLastRecordedAt",
                table: "Location",
                column: "DailyDeskCountLastRecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DailyRoomCountLastRecordedAt",
                table: "Location",
                column: "DailyRoomCountLastRecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Name",
                table: "Location",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Timezone",
                table: "Location",
                column: "Timezone");

            migrationBuilder.CreateIndex(
                name: "IX_JoinInvitation_Role",
                table: "JoinInvitation",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Desk_Deactivated",
                table: "Desk",
                column: "Deactivated");

            migrationBuilder.CreateIndex(
                name: "IX_Desk_Name",
                table: "Desk",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Desk_RequireBookingApproval",
                table: "Desk",
                column: "RequireBookingApproval");

            migrationBuilder.CreateIndex(
                name: "IX_BookingResource_ResourcesId",
                table: "BookingResource",
                column: "ResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_DeletedAt",
                table: "OrganizationResourceType",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_Name",
                table: "OrganizationResourceType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_OrganizationId",
                table: "OrganizationResourceType",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationResourceType_Type",
                table: "OrganizationResourceType",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagResource_ResourcesId",
                table: "OrganizationTagResource",
                column: "ResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Deactivated",
                table: "Resource",
                column: "Deactivated");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_DeletedAt",
                table: "Resource",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_LocationId",
                table: "Resource",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Name",
                table: "Resource",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_OrganizationResourceTypeId",
                table: "Resource",
                column: "OrganizationResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_RequireBookingApproval",
                table: "Resource",
                column: "RequireBookingApproval");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingResource");

            migrationBuilder.DropTable(
                name: "OrganizationTagResource");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "OrganizationResourceType");

            migrationBuilder.DropIndex(
                name: "IX_Room_Deactivated",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_Name",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_RequireBookingApproval",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationTag_Type",
                table: "OrganizationTag");

            migrationBuilder.DropIndex(
                name: "IX_Location_About",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_DailyDeskCountLastRecordedAt",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_DailyRoomCountLastRecordedAt",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_Name",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_Timezone",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_JoinInvitation_Role",
                table: "JoinInvitation");

            migrationBuilder.DropIndex(
                name: "IX_Desk_Deactivated",
                table: "Desk");

            migrationBuilder.DropIndex(
                name: "IX_Desk_Name",
                table: "Desk");

            migrationBuilder.DropIndex(
                name: "IX_Desk_RequireBookingApproval",
                table: "Desk");
        }
    }
}
