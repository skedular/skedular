using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplaceRefund",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocalEntityType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocalEntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReferenceTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RefundPercentage = table.Column<int>(type: "integer", nullable: false),
                    AppliedRuleMinutesBefore = table.Column<int>(type: "integer", nullable: true),
                    BaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Reason = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RequestedByCustomerId = table.Column<string>(type: "character varying(100)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceRefund", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceRefund_Customer_RequestedByCustomerId",
                        column: x => x.RequestedByCustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketplaceRefund_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_CreatedAt",
                table: "MarketplaceRefund",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_ModifiedAt",
                table: "MarketplaceRefund",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_OrganizationId",
                table: "MarketplaceRefund",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_OrganizationId_LocalEntityType_LocalEntit~",
                table: "MarketplaceRefund",
                columns: new[] { "OrganizationId", "LocalEntityType", "LocalEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_RequestedByCustomerId",
                table: "MarketplaceRefund",
                column: "RequestedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRefund_Status",
                table: "MarketplaceRefund",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceRefund");
        }
    }
}
