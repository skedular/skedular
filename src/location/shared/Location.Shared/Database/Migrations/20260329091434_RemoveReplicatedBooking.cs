using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReplicatedBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingLocation");

            migrationBuilder.DropTable(
                name: "BookingResource");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.CreateTable(
                name: "DailyBookingCountRecording",
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
                    table.PrimaryKey("PK_DailyBookingCountRecording", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyBookingCountRecording_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyDeskBookingCountRecording",
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
                    table.PrimaryKey("PK_DailyDeskBookingCountRecording", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyDeskBookingCountRecording_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyRoomBookingCountRecording",
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
                    table.PrimaryKey("PK_DailyRoomBookingCountRecording", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyRoomBookingCountRecording_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_Count",
                table: "DailyBookingCountRecording",
                column: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_CreatedAt",
                table: "DailyBookingCountRecording",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_Date",
                table: "DailyBookingCountRecording",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_DeletedAt",
                table: "DailyBookingCountRecording",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_LocationId",
                table: "DailyBookingCountRecording",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_ModifiedAt",
                table: "DailyBookingCountRecording",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDeskBookingCountRecording_Count",
                table: "DailyDeskBookingCountRecording",
                column: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDeskBookingCountRecording_CreatedAt",
                table: "DailyDeskBookingCountRecording",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDeskBookingCountRecording_Date",
                table: "DailyDeskBookingCountRecording",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDeskBookingCountRecording_DeletedAt",
                table: "DailyDeskBookingCountRecording",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDeskBookingCountRecording_LocationId",
                table: "DailyDeskBookingCountRecording",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDeskBookingCountRecording_ModifiedAt",
                table: "DailyDeskBookingCountRecording",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomBookingCountRecording_Count",
                table: "DailyRoomBookingCountRecording",
                column: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomBookingCountRecording_CreatedAt",
                table: "DailyRoomBookingCountRecording",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomBookingCountRecording_Date",
                table: "DailyRoomBookingCountRecording",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomBookingCountRecording_DeletedAt",
                table: "DailyRoomBookingCountRecording",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomBookingCountRecording_LocationId",
                table: "DailyRoomBookingCountRecording",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRoomBookingCountRecording_ModifiedAt",
                table: "DailyRoomBookingCountRecording",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyBookingCountRecording");

            migrationBuilder.DropTable(
                name: "DailyDeskBookingCountRecording");

            migrationBuilder.DropTable(
                name: "DailyRoomBookingCountRecording");

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    From = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingLocation",
                columns: table => new
                {
                    InvolvedBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedLocationsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingLocation", x => new { x.InvolvedBookingsId, x.InvolvedLocationsId });
                    table.ForeignKey(
                        name: "FK_BookingLocation_Booking_InvolvedBookingsId",
                        column: x => x.InvolvedBookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingLocation_Location_InvolvedLocationsId",
                        column: x => x.InvolvedLocationsId,
                        principalTable: "Location",
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

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CreatedAt",
                table: "Booking",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_DeletedAt",
                table: "Booking",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_From",
                table: "Booking",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ModifiedAt",
                table: "Booking",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Until",
                table: "Booking",
                column: "Until");

            migrationBuilder.CreateIndex(
                name: "IX_BookingLocation_InvolvedLocationsId",
                table: "BookingLocation",
                column: "InvolvedLocationsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingResource_ResourcesId",
                table: "BookingResource",
                column: "ResourcesId");
        }
    }
}
