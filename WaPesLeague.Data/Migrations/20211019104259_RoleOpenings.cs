using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class RoleOpenings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerRoles",
                columns: table => new
                {
                    ServerRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    DiscordRoleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerRoles", x => x.ServerRoleId);
                    table.ForeignKey(
                        name: "FK_ServerRoles_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixGroupRoleOpenings",
                columns: table => new
                {
                    MixGroupRoleOpeningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerRoleId = table.Column<int>(type: "int", nullable: false),
                    MixGroupId = table.Column<int>(type: "int", nullable: false),
                    Minutes = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixGroupRoleOpenings", x => x.MixGroupRoleOpeningId);
                    table.ForeignKey(
                        name: "FK_MixGroupRoleOpenings_MixGroups_MixGroupId",
                        column: x => x.MixGroupId,
                        principalTable: "MixGroups",
                        principalColumn: "MixGroupId");
                    table.ForeignKey(
                        name: "FK_MixGroupRoleOpenings_ServerRoles_ServerRoleId",
                        column: x => x.ServerRoleId,
                        principalTable: "ServerRoles",
                        principalColumn: "ServerRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixTeamRoleOpenings",
                columns: table => new
                {
                    MixTeamRoleOpeningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerRoleId = table.Column<int>(type: "int", nullable: false),
                    MixTeamId = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixTeamRoleOpenings", x => x.MixTeamRoleOpeningId);
                    table.ForeignKey(
                        name: "FK_MixTeamRoleOpenings_MixTeams_MixTeamId",
                        column: x => x.MixTeamId,
                        principalTable: "MixTeams",
                        principalColumn: "MixTeamId");
                    table.ForeignKey(
                        name: "FK_MixTeamRoleOpenings_ServerRoles_ServerRoleId",
                        column: x => x.ServerRoleId,
                        principalTable: "ServerRoles",
                        principalColumn: "ServerRoleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MixGroupRoleOpenings_MixGroupId",
                table: "MixGroupRoleOpenings",
                column: "MixGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MixGroupRoleOpenings_ServerRoleId",
                table: "MixGroupRoleOpenings",
                column: "ServerRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MixTeamRoleOpenings_MixTeamId",
                table: "MixTeamRoleOpenings",
                column: "MixTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MixTeamRoleOpenings_ServerRoleId",
                table: "MixTeamRoleOpenings",
                column: "ServerRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Server_DiscordServerRoleIdId",
                table: "ServerRoles",
                column: "DiscordRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerRoles_ServerId",
                table: "ServerRoles",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MixGroupRoleOpenings");

            migrationBuilder.DropTable(
                name: "MixTeamRoleOpenings");

            migrationBuilder.DropTable(
                name: "ServerRoles");
        }
    }
}
