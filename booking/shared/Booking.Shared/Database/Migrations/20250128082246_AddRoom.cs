using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RequireBookingApproval = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
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
                name: "CustomerRoom",
                columns: table => new
                {
                    PreferredByCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    PreferredRoomsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRoom", x => new { x.PreferredByCustomersId, x.PreferredRoomsId });
                    table.ForeignKey(
                        name: "FK_CustomerRoom_Customer_PreferredByCustomersId",
                        column: x => x.PreferredByCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerRoom_Room_PreferredRoomsId",
                        column: x => x.PreferredRoomsId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTagRoom",
                columns: table => new
                {
                    OrganizationTagsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    TaggedRoomsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTagRoom", x => new { x.OrganizationTagsId, x.TaggedRoomsId });
                    table.ForeignKey(
                        name: "FK_OrganizationTagRoom_OrganizationTag_OrganizationTagsId",
                        column: x => x.OrganizationTagsId,
                        principalTable: "OrganizationTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTagRoom_Room_TaggedRoomsId",
                        column: x => x.TaggedRoomsId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRoom_RoomsId",
                table: "BookingRoom",
                column: "RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRoom_PreferredRoomsId",
                table: "CustomerRoom",
                column: "PreferredRoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTagRoom_TaggedRoomsId",
                table: "OrganizationTagRoom",
                column: "TaggedRoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_DeletedAt",
                table: "Room",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Room_LocationId",
                table: "Room",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_Name",
                table: "Room",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingRoom");

            migrationBuilder.DropTable(
                name: "CustomerRoom");

            migrationBuilder.DropTable(
                name: "OrganizationTagRoom");

            migrationBuilder.DropTable(
                name: "Room");
        }
    }
}
