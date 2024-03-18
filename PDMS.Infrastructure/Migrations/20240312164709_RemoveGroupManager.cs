using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDMS.Infrastructure.Migrations
{
    public partial class RemoveGroupManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Employees_ManagerId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ManagerId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Groups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ManagerId",
                table: "Groups",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Employees_ManagerId",
                table: "Groups",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "EmpId");
        }
    }
}
