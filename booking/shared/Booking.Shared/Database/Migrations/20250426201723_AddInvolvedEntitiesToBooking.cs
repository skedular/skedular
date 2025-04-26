using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInvolvedEntitiesToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingCustomer",
                columns: table => new
                {
                    InvolvedBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedCustomersId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCustomer", x => new { x.InvolvedBookingsId, x.InvolvedCustomersId });
                    table.ForeignKey(
                        name: "FK_BookingCustomer_Booking_InvolvedBookingsId",
                        column: x => x.InvolvedBookingsId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingCustomer_Customer_InvolvedCustomersId",
                        column: x => x.InvolvedCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_BookingCustomer_InvolvedCustomersId",
                table: "BookingCustomer",
                column: "InvolvedCustomersId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingLocation_InvolvedLocationsId",
                table: "BookingLocation",
                column: "InvolvedLocationsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingOrganization_InvolvedOrganizationsId",
                table: "BookingOrganization",
                column: "InvolvedOrganizationsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTeam_InvolvedTeamsId",
                table: "BookingTeam",
                column: "InvolvedTeamsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingCustomer");

            migrationBuilder.DropTable(
                name: "BookingLocation");

            migrationBuilder.DropTable(
                name: "BookingOrganization");

            migrationBuilder.DropTable(
                name: "BookingTeam");
        }
    }
}
