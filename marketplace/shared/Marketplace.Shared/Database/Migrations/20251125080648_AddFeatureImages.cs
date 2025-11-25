using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<CdnImageFile>>(
                name: "FeatureImages",
                table: "ProductVersion",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<ICollection<CdnImageFile>>(
                name: "FeatureImages",
                table: "Product",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureImages",
                table: "ProductVersion");

            migrationBuilder.DropColumn(
                name: "FeatureImages",
                table: "Product");
        }
    }
}
