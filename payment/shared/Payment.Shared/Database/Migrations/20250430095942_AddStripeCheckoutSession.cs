using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeCheckoutSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "Booking",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StripeCheckoutSession",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StripeCheckoutSessionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StripeCustomerCustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeCheckoutSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeCheckoutSession_StripeCustomer_StripeCustomerCustomer~",
                        column: x => x.StripeCustomerCustomerId,
                        principalTable: "StripeCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_StripeCheckoutSessionId",
                table: "Booking",
                column: "StripeCheckoutSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_CreatedAt",
                table: "StripeCheckoutSession",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_ModifiedAt",
                table: "StripeCheckoutSession",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_PaymentStatus",
                table: "StripeCheckoutSession",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "StripeCheckoutSession",
                column: "StripeCheckoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeCheckoutSession_StripeCustomerCustomerId",
                table: "StripeCheckoutSession",
                column: "StripeCustomerCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking",
                column: "StripeCheckoutSessionId",
                principalTable: "StripeCheckoutSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_StripeCheckoutSession_StripeCheckoutSessionId",
                table: "Booking");

            migrationBuilder.DropTable(
                name: "StripeCheckoutSession");

            migrationBuilder.DropIndex(
                name: "IX_Booking_StripeCheckoutSessionId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "StripeCheckoutSessionId",
                table: "Booking");
        }
    }
}
