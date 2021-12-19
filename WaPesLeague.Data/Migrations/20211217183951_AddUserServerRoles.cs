using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddUserServerRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMemberServerRole",
                columns: table => new
                {
                    UserMemberServerRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserMemberId = table.Column<int>(type: "int", nullable: false),
                    ServerRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMemberServerRole", x => x.UserMemberServerRoleId);
                    table.ForeignKey(
                        name: "FK_UserMemberServerRole_ServerRoles_ServerRoleId",
                        column: x => x.ServerRoleId,
                        principalTable: "ServerRoles",
                        principalColumn: "ServerRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMemberServerRole_UserMembers_UserMemberId",
                        column: x => x.UserMemberId,
                        principalTable: "UserMembers",
                        principalColumn: "UserMemberId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMemberServerRole_ServerRoleId",
                table: "UserMemberServerRole",
                column: "ServerRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMemberServerRole_UserMemberId",
                table: "UserMemberServerRole",
                column: "UserMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMemberServerRole");
        }
    }
}
