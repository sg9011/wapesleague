using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddUserMemberServerRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberServerRole_ServerRoles_ServerRoleId",
                table: "UserMemberServerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberServerRole_UserMembers_UserMemberId",
                table: "UserMemberServerRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMemberServerRole",
                table: "UserMemberServerRole");

            migrationBuilder.RenameTable(
                name: "UserMemberServerRole",
                newName: "UserMemberServerRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberServerRole_UserMemberId",
                table: "UserMemberServerRoles",
                newName: "IX_UserMemberServerRoles_UserMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberServerRole_ServerRoleId",
                table: "UserMemberServerRoles",
                newName: "IX_UserMemberServerRoles_ServerRoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMemberServerRoles",
                table: "UserMemberServerRoles",
                column: "UserMemberServerRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberServerRoles_ServerRoles_ServerRoleId",
                table: "UserMemberServerRoles",
                column: "ServerRoleId",
                principalTable: "ServerRoles",
                principalColumn: "ServerRoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberServerRoles_UserMembers_UserMemberId",
                table: "UserMemberServerRoles",
                column: "UserMemberId",
                principalTable: "UserMembers",
                principalColumn: "UserMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberServerRoles_ServerRoles_ServerRoleId",
                table: "UserMemberServerRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberServerRoles_UserMembers_UserMemberId",
                table: "UserMemberServerRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMemberServerRoles",
                table: "UserMemberServerRoles");

            migrationBuilder.RenameTable(
                name: "UserMemberServerRoles",
                newName: "UserMemberServerRole");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberServerRoles_UserMemberId",
                table: "UserMemberServerRole",
                newName: "IX_UserMemberServerRole_UserMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberServerRoles_ServerRoleId",
                table: "UserMemberServerRole",
                newName: "IX_UserMemberServerRole_ServerRoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMemberServerRole",
                table: "UserMemberServerRole",
                column: "UserMemberServerRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberServerRole_ServerRoles_ServerRoleId",
                table: "UserMemberServerRole",
                column: "ServerRoleId",
                principalTable: "ServerRoles",
                principalColumn: "ServerRoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberServerRole_UserMembers_UserMemberId",
                table: "UserMemberServerRole",
                column: "UserMemberId",
                principalTable: "UserMembers",
                principalColumn: "UserMemberId");
        }
    }
}
