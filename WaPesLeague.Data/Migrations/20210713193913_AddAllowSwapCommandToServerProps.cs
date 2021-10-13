using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddAllowSwapCommandToServerProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowActiveSwapCommand",
                table: "Servers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowInactiveSwapCommand",
                table: "Servers",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowActiveSwapCommand",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "AllowInactiveSwapCommand",
                table: "Servers");
        }
    }
}
