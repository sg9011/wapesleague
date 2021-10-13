using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddUserMixStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateStatsCalculated",
                table: "MixSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MixUserPositionSessionStats",
                columns: table => new
                {
                    MixUserPositionSessionStatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    MixSessionId = table.Column<int>(type: "int", nullable: false),
                    PlayTimeSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixUserPositionSessionStats", x => x.MixUserPositionSessionStatId);
                    table.ForeignKey(
                        name: "FK_MixUserPositionSessionStats_MixSessions_MixSessionId",
                        column: x => x.MixSessionId,
                        principalTable: "MixSessions",
                        principalColumn: "MixSessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MixUserPositionSessionStats_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MixUserPositionSessionStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MixUserPositionSessionStats_MixSessionId",
                table: "MixUserPositionSessionStats",
                column: "MixSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MixUserPositionSessionStats_PositionId",
                table: "MixUserPositionSessionStats",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_MixUserPositionSessionStats_UserId",
                table: "MixUserPositionSessionStats",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MixUserPositionSessionStats");

            migrationBuilder.DropColumn(
                name: "DateStatsCalculated",
                table: "MixSessions");
        }
    }
}
