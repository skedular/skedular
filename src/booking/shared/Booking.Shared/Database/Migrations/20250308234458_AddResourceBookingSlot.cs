using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceBookingSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceBookingSlot",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    ResourceId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceBookingSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceBookingSlot_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerResourceBookingSlot",
                columns: table => new
                {
                    CustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ResourceBookingSlotsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerResourceBookingSlot", x => new { x.CustomersId, x.ResourceBookingSlotsId });
                    table.ForeignKey(
                        name: "FK_CustomerResourceBookingSlot_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerResourceBookingSlot_ResourceBookingSlot_ResourceBoo~",
                        column: x => x.ResourceBookingSlotsId,
                        principalTable: "ResourceBookingSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerResourceBookingSlot_ResourceBookingSlotsId",
                table: "CustomerResourceBookingSlot",
                column: "ResourceBookingSlotsId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBookingSlot_Available",
                table: "ResourceBookingSlot",
                column: "Available");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBookingSlot_ResourceId",
                table: "ResourceBookingSlot",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBookingSlot_Start",
                table: "ResourceBookingSlot",
                column: "Start");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerResourceBookingSlot");

            migrationBuilder.DropTable(
                name: "ResourceBookingSlot");
        }
    }
}
