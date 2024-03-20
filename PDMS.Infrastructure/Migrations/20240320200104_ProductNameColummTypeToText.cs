using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDMS.Infrastructure.Migrations
{
    public partial class ProductNameColummTypeToText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Products",
                type: "nvarchar",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
