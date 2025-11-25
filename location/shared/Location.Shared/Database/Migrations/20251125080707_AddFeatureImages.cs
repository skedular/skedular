using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<CdnImageFile>>(
                name: "FeatureImages",
                table: "Location",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureImages",
                table: "Location");
        }
    }
}
