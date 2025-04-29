using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class MadeLineItemMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<ProductVersionLineItem>>(
                name: "LineItems",
                table: "Booking",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<ProductVersionLineItem>),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<ProductVersionLineItem>>(
                name: "LineItems",
                table: "Booking",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(ICollection<ProductVersionLineItem>),
                oldType: "jsonb");
        }
    }
}
