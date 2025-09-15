using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mimarlik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAreaPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 13, 34, 54, 456, DateTimeKind.Utc).AddTicks(8107), new DateTime(2025, 9, 15, 13, 34, 54, 456, DateTimeKind.Utc).AddTicks(8108) });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 13, 34, 54, 456, DateTimeKind.Utc).AddTicks(7815), new DateTime(2025, 9, 15, 13, 34, 54, 456, DateTimeKind.Utc).AddTicks(7822) });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 13, 34, 54, 456, DateTimeKind.Utc).AddTicks(7828), new DateTime(2025, 9, 15, 13, 34, 54, 456, DateTimeKind.Utc).AddTicks(7828) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 13, 34, 4, 235, DateTimeKind.Utc).AddTicks(8454), new DateTime(2025, 9, 15, 13, 34, 4, 235, DateTimeKind.Utc).AddTicks(8454) });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 13, 34, 4, 235, DateTimeKind.Utc).AddTicks(8302), new DateTime(2025, 9, 15, 13, 34, 4, 235, DateTimeKind.Utc).AddTicks(8305) });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 13, 34, 4, 235, DateTimeKind.Utc).AddTicks(8308), new DateTime(2025, 9, 15, 13, 34, 4, 235, DateTimeKind.Utc).AddTicks(8309) });
        }
    }
}
