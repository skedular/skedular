using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestedResourcesToBothRecurringBookingAndMarketplaceBookingSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplaceBookingSubscriptionResource",
                columns: table => new
                {
                    RequestedByMarketplaceBookingSubscriptionsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RequestedResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingSubscriptionResource", x => new { x.RequestedByMarketplaceBookingSubscriptionsId, x.RequestedResourcesId });
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscriptionResource_MarketplaceBookingSu~",
                        column: x => x.RequestedByMarketplaceBookingSubscriptionsId,
                        principalTable: "MarketplaceBookingSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscriptionResource_Resource_RequestedRe~",
                        column: x => x.RequestedResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBookingResource",
                columns: table => new
                {
                    RequestedByRecurringBookingsId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RequestedResourcesId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBookingResource", x => new { x.RequestedByRecurringBookingsId, x.RequestedResourcesId });
                    table.ForeignKey(
                        name: "FK_RecurringBookingResource_RecurringBooking_RequestedByRecurr~",
                        column: x => x.RequestedByRecurringBookingsId,
                        principalTable: "RecurringBooking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBookingResource_Resource_RequestedResourcesId",
                        column: x => x.RequestedResourcesId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscriptionResource_RequestedResourcesId",
                table: "MarketplaceBookingSubscriptionResource",
                column: "RequestedResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBookingResource_RequestedResourcesId",
                table: "RecurringBookingResource",
                column: "RequestedResourcesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceBookingSubscriptionResource");

            migrationBuilder.DropTable(
                name: "RecurringBookingResource");
        }
    }
}
