using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddSnipersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Snipers",
                columns: table => new
                {
                    SniperId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserMemberId = table.Column<int>(type: "int", nullable: false),
                    InitiatedByServerSnipingId = table.Column<int>(type: "int", nullable: false),
                    CatchedOnMixSessionId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snipers", x => x.SniperId);
                    table.ForeignKey(
                        name: "FK_Snipers_MixSessions_CatchedOnMixSessionId",
                        column: x => x.CatchedOnMixSessionId,
                        principalTable: "MixSessions",
                        principalColumn: "MixSessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Snipers_ServerSnipings_InitiatedByServerSnipingId",
                        column: x => x.InitiatedByServerSnipingId,
                        principalTable: "ServerSnipings",
                        principalColumn: "ServerSnipingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Snipers_UserMembers_UserMemberId",
                        column: x => x.UserMemberId,
                        principalTable: "UserMembers",
                        principalColumn: "UserMemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Snipers_CatchedOnMixSessionId",
                table: "Snipers",
                column: "CatchedOnMixSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Snipers_InitiatedByServerSnipingId",
                table: "Snipers",
                column: "InitiatedByServerSnipingId");

            migrationBuilder.CreateIndex(
                name: "IX_Snipers_UserMemberId",
                table: "Snipers",
                column: "UserMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Snipers");
        }
    }
}
