using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedOrderDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "s_Description",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                computedColumnSql: "'Data utworzenia zamówienia: ' + Cast([DateTime] as varchar(250))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "s_Description",
                table: "Orders");
        }
    }
}
