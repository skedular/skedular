using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReplicatedBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingTeam");

            migrationBuilder.DropTable(
                name: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "BookingTeam",
                columns: table => new
                {
                    InvolvedBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedTeamsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTeam", x => new { x.InvolvedBookingsId, x.InvolvedTeamsId });
                    table.ForeignKey(
                        name: "FK_BookingTeam_Booking_InvolvedBookingsId",
                        column: x => x.InvolvedBookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTeam_Team_InvolvedTeamsId",
                        column: x => x.InvolvedTeamsId,
                        principalTable: "Team",
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
                name: "IX_BookingTeam_InvolvedTeamsId",
                table: "BookingTeam",
                column: "InvolvedTeamsId");
        }
    }
}
