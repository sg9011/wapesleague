using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddServerEventsAndUserDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DiscordJoin",
                table: "UserMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServerJoin",
                table: "UserMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServerEvent",
                columns: table => new
                {
                    ServerEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionEntity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerEvent", x => x.ServerEventId);
                    table.ForeignKey(
                        name: "FK_ServerEvent_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerEvent_ServerId",
                table: "ServerEvent",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerEvent");

            migrationBuilder.DropColumn(
                name: "DiscordJoin",
                table: "UserMembers");

            migrationBuilder.DropColumn(
                name: "ServerJoin",
                table: "UserMembers");
        }
    }
}
