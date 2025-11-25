using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<CdnImageFile>>(
                name: "FeatureImages",
                table: "Team",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureImages",
                table: "Team");
        }
    }
}
