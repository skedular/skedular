using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingPaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingCheckoutSession",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PaymentReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CheckoutUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BookingId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCheckoutSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingCheckoutSession_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_BookingId",
                table: "BookingCheckoutSession",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_CreatedAt",
                table: "BookingCheckoutSession",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_ModifiedAt",
                table: "BookingCheckoutSession",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_PaymentReferenceId",
                table: "BookingCheckoutSession",
                column: "PaymentReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCheckoutSession_PaymentStatus",
                table: "BookingCheckoutSession",
                column: "PaymentStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingCheckoutSession");
        }
    }
}
