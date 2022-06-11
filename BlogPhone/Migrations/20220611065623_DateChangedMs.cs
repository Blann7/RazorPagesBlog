using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPhone.Migrations
{
    public partial class DateChangedMs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleValidityDate",
                table: "Users");

            migrationBuilder.AddColumn<long>(
                name: "BanMs",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoleValidityMs",
                table: "Users",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanMs",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleValidityMs",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "BanDate",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleValidityDate",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
