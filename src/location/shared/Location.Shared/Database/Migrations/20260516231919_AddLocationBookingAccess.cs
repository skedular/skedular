using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationBookingAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationBookingAccess",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActiveBookingCount = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EventRaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationBookingAccess", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_ActiveBookingCount",
                table: "LocationBookingAccess",
                column: "ActiveBookingCount");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_CreatedAt",
                table: "LocationBookingAccess",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_CustomerId",
                table: "LocationBookingAccess",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_CustomerId_LocationId",
                table: "LocationBookingAccess",
                columns: new[] { "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_CustomerId_LocationId_OrganizationId",
                table: "LocationBookingAccess",
                columns: new[] { "CustomerId", "LocationId", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_CustomerId_OrganizationId",
                table: "LocationBookingAccess",
                columns: new[] { "CustomerId", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_DeletedAt",
                table: "LocationBookingAccess",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_LocationId",
                table: "LocationBookingAccess",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_ModifiedAt",
                table: "LocationBookingAccess",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookingAccess_OrganizationId",
                table: "LocationBookingAccess",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationBookingAccess");
        }
    }
}
