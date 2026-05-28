using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationMember");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationMember",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationMember_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationMember_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CreatedAt",
                table: "LocationMember",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_CustomerId_LocationId",
                table: "LocationMember",
                columns: new[] { "CustomerId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_DeletedAt",
                table: "LocationMember",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_LocationId",
                table: "LocationMember",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_ModifiedAt",
                table: "LocationMember",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMember_Role",
                table: "LocationMember",
                column: "Role");
        }
    }
}
