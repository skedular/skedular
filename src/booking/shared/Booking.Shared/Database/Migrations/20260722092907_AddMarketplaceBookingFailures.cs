using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceBookingFailures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplaceBookingFailure",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FailureKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Scope = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FinalizedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RequestedFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequestedUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequestedResourceIds = table.Column<string>(type: "jsonb", nullable: false),
                    CustomerAction = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Reason = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    BookingId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RecurringBookingId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MarketplaceBookingSubscriptionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingFailure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingFailureDelivery",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarketplaceBookingFailureId = table.Column<string>(type: "character varying(100)", nullable: false),
                    RecipientKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecipientCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RecipientEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    Audience = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Channel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    LastAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingFailureDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingFailureDelivery_MarketplaceBookingFailure~",
                        column: x => x.MarketplaceBookingFailureId,
                        principalTable: "MarketplaceBookingFailure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceBookingFailureEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarketplaceBookingFailureId = table.Column<string>(type: "character varying(100)", nullable: false),
                    EventType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(100000)", maxLength: 100000, nullable: true),
                    LastError = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    ActorCustomerId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceBookingFailureEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceBookingFailureEvent_MarketplaceBookingFailure_Ma~",
                        column: x => x.MarketplaceBookingFailureId,
                        principalTable: "MarketplaceBookingFailure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_BookingId",
                table: "MarketplaceBookingFailure",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_CreatedAt",
                table: "MarketplaceBookingFailure",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_FailureKey",
                table: "MarketplaceBookingFailure",
                column: "FailureKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_FinalizedAt",
                table: "MarketplaceBookingFailure",
                column: "FinalizedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_MarketplaceBookingSubscriptionId",
                table: "MarketplaceBookingFailure",
                column: "MarketplaceBookingSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_ModifiedAt",
                table: "MarketplaceBookingFailure",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailure_RecurringBookingId",
                table: "MarketplaceBookingFailure",
                column: "RecurringBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureDelivery_CreatedAt",
                table: "MarketplaceBookingFailureDelivery",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureDelivery_MarketplaceBookingFailure~",
                table: "MarketplaceBookingFailureDelivery",
                columns: new[] { "MarketplaceBookingFailureId", "RecipientKey", "Channel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureDelivery_ModifiedAt",
                table: "MarketplaceBookingFailureDelivery",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureDelivery_Status_LastAttemptAt",
                table: "MarketplaceBookingFailureDelivery",
                columns: new[] { "Status", "LastAttemptAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureEvent_CreatedAt",
                table: "MarketplaceBookingFailureEvent",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureEvent_MarketplaceBookingFailureId_~",
                table: "MarketplaceBookingFailureEvent",
                columns: new[] { "MarketplaceBookingFailureId", "OccurredAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceBookingFailureEvent_ModifiedAt",
                table: "MarketplaceBookingFailureEvent",
                column: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceBookingFailureDelivery");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingFailureEvent");

            migrationBuilder.DropTable(
                name: "MarketplaceBookingFailure");
        }
    }
}
