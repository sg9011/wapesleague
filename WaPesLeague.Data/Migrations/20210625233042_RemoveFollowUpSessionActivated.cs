using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class RemoveFollowUpSessionActivated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpSessionActivated",
                table: "MixSessions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FollowUpSessionActivated",
                table: "MixSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
