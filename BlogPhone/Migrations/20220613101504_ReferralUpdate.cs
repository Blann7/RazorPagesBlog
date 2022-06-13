using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPhone.Migrations
{
    public partial class ReferralUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvitedUsers",
                table: "Referrals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitedUsers",
                table: "Referrals");
        }
    }
}
