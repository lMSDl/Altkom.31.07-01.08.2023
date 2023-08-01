using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "OrderNumber",
                startValue: 100L,
                incrementBy: 333,
                minValue: 0L,
                maxValue: 999L,
                cyclic: true);

            migrationBuilder.AlterColumn<string>(
                name: "s_Number",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "STR(NEXT VALUE FOR OrderNumber)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "OrderNumber");

            migrationBuilder.AlterColumn<string>(
                name: "s_Number",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "STR(NEXT VALUE FOR OrderNumber)");
        }
    }
}
