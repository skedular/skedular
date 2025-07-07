using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDefaultValueForAcceptedBookingPaymentMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb",
                oldDefaultValue: new string[0]);

            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "Product",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb",
                oldDefaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<ICollection<string>>(
                name: "AcceptedBookingPaymentMethods",
                table: "Product",
                type: "jsonb",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(ICollection<string>),
                oldType: "jsonb");
        }
    }
}
