using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReplicatedBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingOrganization");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.CreateTable(
                name: "DailyBookingCountRecording",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyBookingCountRecording", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyBookingCountRecording_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
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
                name: "IX_DailyBookingCountRecording_ModifiedAt",
                table: "DailyBookingCountRecording",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBookingCountRecording_OrganizationId",
                table: "DailyBookingCountRecording",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyBookingCountRecording");

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
                name: "BookingOrganization",
                columns: table => new
                {
                    InvolvedBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedOrganizationsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingOrganization", x => new { x.InvolvedBookingsId, x.InvolvedOrganizationsId });
                    table.ForeignKey(
                        name: "FK_BookingOrganization_Booking_InvolvedBookingsId",
                        column: x => x.InvolvedBookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingOrganization_Organization_InvolvedOrganizationsId",
                        column: x => x.InvolvedOrganizationsId,
                        principalTable: "Organization",
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
                name: "IX_BookingOrganization_InvolvedOrganizationsId",
                table: "BookingOrganization",
                column: "InvolvedOrganizationsId");
        }
    }
}
