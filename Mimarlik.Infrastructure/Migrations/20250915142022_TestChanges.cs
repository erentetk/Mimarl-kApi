using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mimarlik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7729), new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7730) });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7529), new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7532) });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7536), new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7536) });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "CreatedAt", "IsDefault", "Name", "NativeName", "SortOrder", "Status", "UpdatedAt" },
                values: new object[] { 3, "de", new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7539), false, "Deutsch", "Deutsch", 3, 1, new DateTime(2025, 9, 15, 14, 20, 21, 834, DateTimeKind.Utc).AddTicks(7539) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 3);

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
    }
}
