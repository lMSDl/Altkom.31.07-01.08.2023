using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedStoredOrderDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "s_Number",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "s_Description",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                computedColumnSql: "'Data utworzenia zamówienia: ' + [s_Number]",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComputedColumnSql: "'Data utworzenia zamówienia: ' + Cast([DateTime] as varchar(250))",
                oldStored: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "s_Number",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "s_Description",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                computedColumnSql: "'Data utworzenia zamówienia: ' + Cast([DateTime] as varchar(250))",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComputedColumnSql: "'Data utworzenia zamówienia: ' + [s_Number]");
        }
    }
}
