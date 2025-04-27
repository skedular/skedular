using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInvolvedEntitiesToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_BookingOrganization_InvolvedOrganizationsId",
                table: "BookingOrganization",
                column: "InvolvedOrganizationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingOrganization");
        }
    }
}
