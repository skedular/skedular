using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeAcceptedBookingPaymentMethodsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
