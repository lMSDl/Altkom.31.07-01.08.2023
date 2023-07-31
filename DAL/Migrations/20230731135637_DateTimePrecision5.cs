using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DateTimePrecision5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Products",
                type: "datetime2(5)",
                precision: 5,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldPrecision: 7);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Orders",
                type: "datetime2(5)",
                precision: 5,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldPrecision: 7);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "Orders",
                type: "datetime2(5)",
                precision: 5,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldPrecision: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Products",
                type: "datetime2(7)",
                precision: 7,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(5)",
                oldPrecision: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Orders",
                type: "datetime2(7)",
                precision: 7,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(5)",
                oldPrecision: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "Orders",
                type: "datetime2(7)",
                precision: 7,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(5)",
                oldPrecision: 5);
        }
    }
}
