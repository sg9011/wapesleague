using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class RenameServerEventToServerEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerEvent_Servers_ServerId",
                table: "ServerEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerEvent",
                table: "ServerEvent");

            migrationBuilder.RenameTable(
                name: "ServerEvent",
                newName: "ServerEvents");

            migrationBuilder.RenameIndex(
                name: "IX_ServerEvent_ServerId",
                table: "ServerEvents",
                newName: "IX_ServerEvents_ServerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerEvents",
                table: "ServerEvents",
                column: "ServerEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerEvents_Servers_ServerId",
                table: "ServerEvents",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerEvents_Servers_ServerId",
                table: "ServerEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerEvents",
                table: "ServerEvents");

            migrationBuilder.RenameTable(
                name: "ServerEvents",
                newName: "ServerEvent");

            migrationBuilder.RenameIndex(
                name: "IX_ServerEvents_ServerId",
                table: "ServerEvent",
                newName: "IX_ServerEvent_ServerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerEvent",
                table: "ServerEvent",
                column: "ServerEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerEvent_Servers_ServerId",
                table: "ServerEvent",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
