using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredTeamPrimaryFeatureImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImageUrl",
                table: "Team");

            migrationBuilder.AddColumn<CdnImageFile>(
                name: "PrimaryFeatureImage",
                table: "Team",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryFeatureImage",
                table: "Team");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryFeatureImageUrl",
                table: "Team",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
