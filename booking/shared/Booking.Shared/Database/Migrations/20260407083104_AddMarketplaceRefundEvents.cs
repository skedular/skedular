using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceRefundEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplaceRefundEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Reason = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    AccountingProvider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ExternalRefundId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExternalRefundNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    MarketplaceRefundId = table.Column<string>(type: "character varying(100)", nullable: false),
                    ActorCustomerId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceRefundEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceRefundEvent_Customer_ActorCustomerId",
                        column: x => x.ActorCustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketplaceRefundEvent_MarketplaceRefund_MarketplaceRefundId",
                        column: x => x.MarketplaceRefundId,
                        principalTable: "MarketplaceRefund",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundEvent_ActorCustomerId",
                table: "MarketplaceRefundEvent",
                column: "ActorCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundEvent_CreatedAt",
                table: "MarketplaceRefundEvent",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundEvent_MarketplaceRefundId_OccurredAt_Creat~",
                table: "MarketplaceRefundEvent",
                columns: new[] { "MarketplaceRefundId", "OccurredAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefundEvent_ModifiedAt",
                table: "MarketplaceRefundEvent",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceRefundEvent");
        }
    }
}
