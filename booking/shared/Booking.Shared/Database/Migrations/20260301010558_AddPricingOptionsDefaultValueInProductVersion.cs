using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingOptionsDefaultValueInProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<ProductVersionPricingOptions>>(
                name: "PricingOptions",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                defaultValue: new ProductVersionPricingOptions[0],
                oldClrType: typeof(ICollection<ProductVersionPricingOptions>),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ICollection<ProductVersionPricingOptions>>(
                name: "PricingOptions",
                table: "ProductVersion",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(ICollection<ProductVersionPricingOptions>),
                oldType: "jsonb",
                oldDefaultValue: new ProductVersionPricingOptions[0]);
        }
    }
}
