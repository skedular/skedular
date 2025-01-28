using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DailyRoomCountLastRecordedAt",
                table: "Location",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DailyRoomCountRecording",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyRoomCountRecording", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyRoomCountRecording_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    RequireBookingApproval = table.Column<bool>(type: "boolean", nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingRoom",
                columns: table => new
                {
                    BookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RoomsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRoom", x => new { x.BookingsId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_BookingRoom_Booking_BookingsId",
                        column: x => x.BookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingRoom_Room_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagRoom",
                columns: table => new
                {
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RoomsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagRoom", x => new { x.OrganizationTagsId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagRoom_OrganizationTag_OrganizationTagsId",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagRoom_Room_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRoom_RoomsId",
                table: "BookingRoom",
                column: "RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomCountRecording_Count",
                table: "DailyRoomCountRecording",
                column: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomCountRecording_Date",
                table: "DailyRoomCountRecording",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomCountRecording_DeletedAt",
                table: "DailyRoomCountRecording",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomCountRecording_LocationId",
                table: "DailyRoomCountRecording",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagRoom_RoomsId",
                table: "OrganizationTagRoom",
                column: "RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_DeletedAt",
                table: "Room",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Room_LocationId",
                table: "Room",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingRoom");

            migrationBuilder.DropTable(
                name: "DailyRoomCountRecording");

            migrationBuilder.DropTable(
                name: "OrganizationTagRoom");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropColumn(
                name: "DailyRoomCountLastRecordedAt",
                table: "Location");
        }
    }
}
