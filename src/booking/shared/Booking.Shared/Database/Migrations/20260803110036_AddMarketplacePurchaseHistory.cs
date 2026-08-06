using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplacePurchaseHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplacePurchaseHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SourceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductVersionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProductTitle = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PurchasedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActivityAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BookingFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BookingUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SubscriptionStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    CancelAtPeriodEnd = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedByCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    LatestRefundId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LatestRefundStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    MarketplaceBookingId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MarketplaceBookingSubscriptionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplacePurchaseHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_Customer_DeletedByCustomerId",
                        column: x => x.DeletedByCustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_MarketplaceBookingSubscription_M~",
                        column: x => x.MarketplaceBookingSubscriptionId,
                        principalTable: "MarketplaceBookingSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_MarketplaceBooking_MarketplaceBo~",
                        column: x => x.MarketplaceBookingId,
                        principalTable: "MarketplaceBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_MarketplaceRefund_LatestRefundId",
                        column: x => x.LatestRefundId,
                        principalTable: "MarketplaceRefund",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketplacePurchaseHistory_ProductVersion_ProductVersionId",
                        column: x => x.ProductVersionId,
                        principalTable: "ProductVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_CreatedAt",
                table: "MarketplacePurchaseHistory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_CustomerId",
                table: "MarketplacePurchaseHistory",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_DeletedByCustomerId",
                table: "MarketplacePurchaseHistory",
                column: "DeletedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_LatestRefundId",
                table: "MarketplacePurchaseHistory",
                column: "LatestRefundId",
                unique: true,
                filter: "\"LatestRefundId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingId",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingId",
                unique: true,
                filter: "\"MarketplaceBookingId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_MarketplaceBookingSubscriptionId",
                table: "MarketplacePurchaseHistory",
                column: "MarketplaceBookingSubscriptionId",
                unique: true,
                filter: "\"MarketplaceBookingSubscriptionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_ModifiedAt",
                table: "MarketplacePurchaseHistory",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_OrganizationId_ActivityAt_Source~",
                table: "MarketplacePurchaseHistory",
                columns: new[] { "OrganizationId", "ActivityAt", "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_ProductVersionId",
                table: "MarketplacePurchaseHistory",
                column: "ProductVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplacePurchaseHistory_SourceType_SourceId",
                table: "MarketplacePurchaseHistory",
                columns: new[] { "SourceType", "SourceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplacePurchaseHistory");
        }
    }
}
