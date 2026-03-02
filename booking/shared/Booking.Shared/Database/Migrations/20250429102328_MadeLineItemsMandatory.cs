using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeLineItemsMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<ProductLineItem>>(
                name: "LineItems",
                table: "Booking",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<ProductLineItem>),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<ProductLineItem>>(
                name: "LineItems",
                table: "Booking",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(ICollection<ProductLineItem>),
                oldType: "jsonb");
        }
    }
}
