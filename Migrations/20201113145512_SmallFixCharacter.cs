using Microsoft.EntityFrameworkCore.Migrations;

namespace rpg.Migrations
{
    public partial class SmallFixCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoPoints",
                table: "Characters",
                newName: "HitPoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HitPoints",
                table: "Characters",
                newName: "HoPoints");
        }
    }
}
