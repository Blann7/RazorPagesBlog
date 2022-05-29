using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPhone.Migrations
{
    public partial class AddAboutUsPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUsPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    P1_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    P1_Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    P1_ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    P2_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    P2_Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    P2_ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUsPage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUsPage");
        }
    }
}
