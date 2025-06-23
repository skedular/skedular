using System;
using System.Collections.Generic;
using Api.Shared.Services.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoreFloorPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourcePosition_DeletedAt",
                table: "ResourcePosition");

            migrationBuilder.DropIndex(
                name: "IX_FloorPlan_FloorLevel",
                table: "FloorPlan");

            migrationBuilder.DropIndex(
                name: "IX_FloorPlan_IsActive",
                table: "FloorPlan");

            migrationBuilder.DropIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ResourcePosition");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ResourcePosition");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "ResourcePosition");

            migrationBuilder.DropColumn(
                name: "Shape",
                table: "ResourcePosition");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "ResourcePosition");

            migrationBuilder.DropColumn(
                name: "FloorLevel",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "FloorName",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "FloorPlan");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "FloorPlan");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FloorPlan",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<CdnImageFile>(
                name: "Image",
                table: "FloorPlan",
                type: "jsonb",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "FloorPlan");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ResourcePosition",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "ResourcePosition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "Metadata",
                table: "ResourcePosition",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shape",
                table: "ResourcePosition",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "ResourcePosition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FloorPlan",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "FloorLevel",
                table: "FloorPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FloorName",
                table: "FloorPlan",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "FloorPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "FloorPlan",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FloorPlan",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "FloorPlan",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "FloorPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePosition_DeletedAt",
                table: "ResourcePosition",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_FloorLevel",
                table: "FloorPlan",
                column: "FloorLevel");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_IsActive",
                table: "FloorPlan",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlan_LocationId_FloorLevel",
                table: "FloorPlan",
                columns: new[] { "LocationId", "FloorLevel" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }
    }
}
