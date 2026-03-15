using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInvolvedEntityToMarketplaceBookingSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByCustomerId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByCustomerId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByCustomerId",
                table: "MarketplaceBookingSubscription",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerMarketplaceBookingSubscription",
                columns: table => new
                {
                    InvolvedCustomersId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedMarketplaceBookingSubscriptionId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMarketplaceBookingSubscription", x => new { x.InvolvedCustomersId, x.InvolvedMarketplaceBookingSubscriptionId });
                    table.ForeignKey(
                        name: "FK_CustomerMarketplaceBookingSubscription_Customer_InvolvedCus~",
                        column: x => x.InvolvedCustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerMarketplaceBookingSubscription_MarketplaceBookingSu~",
                        column: x => x.InvolvedMarketplaceBookingSubscriptionId,
                        principalTable: "MarketplaceBookingSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingSubscriptionOrganization",
                columns: table => new
                {
                    InvolvedMarketplaceBookingSubscriptionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedOrganizationsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingSubscriptionOrganization", x => new { x.InvolvedMarketplaceBookingSubscriptionId, x.InvolvedOrganizationsId });
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscriptionOrganization_MarketplaceBooki~",
                        column: x => x.InvolvedMarketplaceBookingSubscriptionId,
                        principalTable: "MarketplaceBookingSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscriptionOrganization_Organization_Inv~",
                        column: x => x.InvolvedOrganizationsId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingSubscriptionTeam",
                columns: table => new
                {
                    InvolvedMarketplaceBookingSubscriptionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    InvolvedTeamsId = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingSubscriptionTeam", x => new { x.InvolvedMarketplaceBookingSubscriptionId, x.InvolvedTeamsId });
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscriptionTeam_MarketplaceBookingSubscr~",
                        column: x => x.InvolvedMarketplaceBookingSubscriptionId,
                        principalTable: "MarketplaceBookingSubscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingSubscriptionTeam_Team_InvolvedTeamsId",
                        column: x => x.InvolvedTeamsId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_CreatedByCustomerId",
                table: "MarketplaceBookingSubscription",
                column: "CreatedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_DeletedByCustomerId",
                table: "MarketplaceBookingSubscription",
                column: "DeletedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscription_LastModifiedByCustomerId",
                table: "MarketplaceBookingSubscription",
                column: "LastModifiedByCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMarketplaceBookingSubscription_InvolvedMarketplaceB~",
                table: "CustomerMarketplaceBookingSubscription",
                column: "InvolvedMarketplaceBookingSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscriptionOrganization_InvolvedOrganiza~",
                table: "MarketplaceBookingSubscriptionOrganization",
                column: "InvolvedOrganizationsId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingSubscriptionTeam_InvolvedTeamsId",
                table: "MarketplaceBookingSubscriptionTeam",
                column: "InvolvedTeamsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingSubscription_Customer_CreatedByCustomerId",
                table: "MarketplaceBookingSubscription",
                column: "CreatedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingSubscription_Customer_DeletedByCustomerId",
                table: "MarketplaceBookingSubscription",
                column: "DeletedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceBookingSubscription_Customer_LastModifiedByCusto~",
                table: "MarketplaceBookingSubscription",
                column: "LastModifiedByCustomerId",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingSubscription_Customer_CreatedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingSubscription_Customer_DeletedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceBookingSubscription_Customer_LastModifiedByCusto~",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropTable(
                name: "CustomerMarketplaceBookingSubscription");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingSubscriptionOrganization");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingSubscriptionTeam");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingSubscription_CreatedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingSubscription_DeletedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceBookingSubscription_LastModifiedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "CreatedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "DeletedByCustomerId",
                table: "MarketplaceBookingSubscription");

            migrationBuilder.DropColumn(
                name: "LastModifiedByCustomerId",
                table: "MarketplaceBookingSubscription");
        }
    }
}
