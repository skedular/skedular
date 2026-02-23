using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInvolvedEntitiesToRecurringBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByCustomerId",
                table: "RecurringBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByCustomerId",
                table: "RecurringBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByCustomerId",
                table: "RecurringBooking",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldDefaultValue: "WORKING_FROM_OFFICE");

            migrationBuilder.CreateTable(
                name: "CustomerRecurringBooking",
                columns: table => new
                {
                    InvolvedCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedRecurringBookingId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRecurringBooking", x => new { x.InvolvedCustomersId, x.InvolvedRecurringBookingId });
                    table.ForeignKey(
                        name: "FK_CustomerRecurringBooking_Customer_InvolvedCustomersId",
                        column: x => x.InvolvedCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerRecurringBooking_RecurringBooking_InvolvedRecurring~",
                        column: x => x.InvolvedRecurringBookingId,
                        principalTable: "RecurringBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationRecurringBooking",
                columns: table => new
                {
                    InvolvedOrganizationsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedRecurringBookingId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRecurringBooking", x => new { x.InvolvedOrganizationsId, x.InvolvedRecurringBookingId });
                    table.ForeignKey(
                        name: "FK_OrganizationRecurringBooking_Organization_InvolvedOrganizat~",
                        column: x => x.InvolvedOrganizationsId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationRecurringBooking_RecurringBooking_InvolvedRecur~",
                        column: x => x.InvolvedRecurringBookingId,
                        principalTable: "RecurringBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBookingTeam",
                columns: table => new
                {
                    InvolvedRecurringBookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedTeamsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBookingTeam", x => new { x.InvolvedRecurringBookingId, x.InvolvedTeamsId });
                    table.ForeignKey(
                        name: "FK_RecurringBookingTeam_RecurringBooking_InvolvedRecurringBook~",
                        column: x => x.InvolvedRecurringBookingId,
                        principalTable: "RecurringBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBookingTeam_Team_InvolvedTeamsId",
                        column: x => x.InvolvedTeamsId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_CreatedByCustomerId",
                table: "RecurringBooking",
                column: "CreatedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_DeletedByCustomerId",
                table: "RecurringBooking",
                column: "DeletedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBooking_LastModifiedByCustomerId",
                table: "RecurringBooking",
                column: "LastModifiedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecurringBooking_InvolvedRecurringBookingId",
                table: "CustomerRecurringBooking",
                column: "InvolvedRecurringBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRecurringBooking_InvolvedRecurringBookingId",
                table: "OrganizationRecurringBooking",
                column: "InvolvedRecurringBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBookingTeam_InvolvedTeamsId",
                table: "RecurringBookingTeam",
                column: "InvolvedTeamsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBooking_Customer_CreatedByCustomerId",
                table: "RecurringBooking",
                column: "CreatedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBooking_Customer_DeletedByCustomerId",
                table: "RecurringBooking",
                column: "DeletedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBooking_Customer_LastModifiedByCustomerId",
                table: "RecurringBooking",
                column: "LastModifiedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBooking_Customer_CreatedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBooking_Customer_DeletedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBooking_Customer_LastModifiedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropTable(
                name: "CustomerRecurringBooking");

            migrationBuilder.DropTable(
                name: "OrganizationRecurringBooking");

            migrationBuilder.DropTable(
                name: "RecurringBookingTeam");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_CreatedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_DeletedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBooking_LastModifiedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "CreatedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "DeletedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.DropColumn(
                name: "LastModifiedByCustomerId",
                table: "RecurringBooking");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Booking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "WORKING_FROM_OFFICE",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
