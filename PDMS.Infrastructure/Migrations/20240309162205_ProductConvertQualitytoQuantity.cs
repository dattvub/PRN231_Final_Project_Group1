using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDMS.Infrastructure.Migrations
{
    public partial class ProductConvertQualitytoQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quality",
                table: "Products",
                newName: "Quantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "Quality");
        }
    }
}
