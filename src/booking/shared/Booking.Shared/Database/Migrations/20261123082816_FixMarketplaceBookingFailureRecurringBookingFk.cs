using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations;

/// <inheritdoc />
public partial class _20261123082816_FixMarketplaceBookingFailureRecurringBookingFk : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_MarketplaceBookingFailure_Booking_RecurringBookingId",
            table: "MarketplaceBookingFailure");

        migrationBuilder.AddForeignKey(
            name: "FK_MarketplaceBookingFailure_RecurringBooking_RecurringBooking~",
            table: "MarketplaceBookingFailure",
            column: "RecurringBookingId",
            principalTable: "RecurringBooking",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_MarketplaceBookingFailure_RecurringBooking_RecurringBooking~",
            table: "MarketplaceBookingFailure");

        migrationBuilder.AddForeignKey(
            name: "FK_MarketplaceBookingFailure_Booking_RecurringBookingId",
            table: "MarketplaceBookingFailure",
            column: "RecurringBookingId",
            principalTable: "Booking",
            principalColumn: "Id");
    }
}
