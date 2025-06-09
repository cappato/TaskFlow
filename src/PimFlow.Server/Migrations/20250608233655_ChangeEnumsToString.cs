using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEnumsToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CustomAttributes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Articles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Select" });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Color" });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Text" });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Select" });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Select" });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Boolean" });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633), "Text" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 36, 54, 621, DateTimeKind.Utc).AddTicks(8633));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "CustomAttributes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Articles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 6 });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 8 });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 0 });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 6 });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 6 });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 3 });

            migrationBuilder.UpdateData(
                table: "CustomAttributes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635), 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 8, 23, 35, 48, 954, DateTimeKind.Utc).AddTicks(1635));
        }
    }
}
