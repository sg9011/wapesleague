using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddEnabledToServerSniperTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "ServerSnipings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "ServerSnipings");
        }
    }
}
