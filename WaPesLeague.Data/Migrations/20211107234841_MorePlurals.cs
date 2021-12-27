using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class MorePlurals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeam_Associations_AssociationId",
                table: "AssociationTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTeam_AssociationTeamId",
                table: "AssociationTeamPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTenantPlayer_AssociationTenantPlayerId",
                table: "AssociationTeamPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTenantPlayer_AssociationTenants_AssociationTenantId",
                table: "AssociationTenantPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTenantPlayer_Users_UserId",
                table: "AssociationTenantPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchTeam_AssociationTeam_MatchTeamId",
                table: "MatchTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationTenantPlayer",
                table: "AssociationTenantPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationTeam",
                table: "AssociationTeam");

            migrationBuilder.RenameTable(
                name: "AssociationTenantPlayer",
                newName: "AssociationTenantPlayers");

            migrationBuilder.RenameTable(
                name: "AssociationTeam",
                newName: "AssociationTeams");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationTenantPlayer_UserId",
                table: "AssociationTenantPlayers",
                newName: "IX_AssociationTenantPlayers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationTenantPlayer_AssociationTenantId",
                table: "AssociationTenantPlayers",
                newName: "IX_AssociationTenantPlayers_AssociationTenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationTeam_AssociationId",
                table: "AssociationTeams",
                newName: "IX_AssociationTeams_AssociationId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AssociationTenantPlayers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationTenantPlayers",
                table: "AssociationTenantPlayers",
                column: "AssociationTenantPlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationTeams",
                table: "AssociationTeams",
                column: "AssociationTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTeams_AssociationTeamId",
                table: "AssociationTeamPlayer",
                column: "AssociationTeamId",
                principalTable: "AssociationTeams",
                principalColumn: "AssociationTeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTenantPlayers_AssociationTenantPlayerId",
                table: "AssociationTeamPlayer",
                column: "AssociationTenantPlayerId",
                principalTable: "AssociationTenantPlayers",
                principalColumn: "AssociationTenantPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeams_Associations_AssociationId",
                table: "AssociationTeams",
                column: "AssociationId",
                principalTable: "Associations",
                principalColumn: "AssociationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTenantPlayers_AssociationTenants_AssociationTenantId",
                table: "AssociationTenantPlayers",
                column: "AssociationTenantId",
                principalTable: "AssociationTenants",
                principalColumn: "AssociationTenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTenantPlayers_Users_UserId",
                table: "AssociationTenantPlayers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchTeam_AssociationTeams_MatchTeamId",
                table: "MatchTeam",
                column: "MatchTeamId",
                principalTable: "AssociationTeams",
                principalColumn: "AssociationTeamId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTeams_AssociationTeamId",
                table: "AssociationTeamPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTenantPlayers_AssociationTenantPlayerId",
                table: "AssociationTeamPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeams_Associations_AssociationId",
                table: "AssociationTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTenantPlayers_AssociationTenants_AssociationTenantId",
                table: "AssociationTenantPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTenantPlayers_Users_UserId",
                table: "AssociationTenantPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchTeam_AssociationTeams_MatchTeamId",
                table: "MatchTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationTenantPlayers",
                table: "AssociationTenantPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationTeams",
                table: "AssociationTeams");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AssociationTenantPlayers");

            migrationBuilder.RenameTable(
                name: "AssociationTenantPlayers",
                newName: "AssociationTenantPlayer");

            migrationBuilder.RenameTable(
                name: "AssociationTeams",
                newName: "AssociationTeam");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationTenantPlayers_UserId",
                table: "AssociationTenantPlayer",
                newName: "IX_AssociationTenantPlayer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationTenantPlayers_AssociationTenantId",
                table: "AssociationTenantPlayer",
                newName: "IX_AssociationTenantPlayer_AssociationTenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationTeams_AssociationId",
                table: "AssociationTeam",
                newName: "IX_AssociationTeam_AssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationTenantPlayer",
                table: "AssociationTenantPlayer",
                column: "AssociationTenantPlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationTeam",
                table: "AssociationTeam",
                column: "AssociationTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeam_Associations_AssociationId",
                table: "AssociationTeam",
                column: "AssociationId",
                principalTable: "Associations",
                principalColumn: "AssociationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTeam_AssociationTeamId",
                table: "AssociationTeamPlayer",
                column: "AssociationTeamId",
                principalTable: "AssociationTeam",
                principalColumn: "AssociationTeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeamPlayer_AssociationTenantPlayer_AssociationTenantPlayerId",
                table: "AssociationTeamPlayer",
                column: "AssociationTenantPlayerId",
                principalTable: "AssociationTenantPlayer",
                principalColumn: "AssociationTenantPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTenantPlayer_AssociationTenants_AssociationTenantId",
                table: "AssociationTenantPlayer",
                column: "AssociationTenantId",
                principalTable: "AssociationTenants",
                principalColumn: "AssociationTenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTenantPlayer_Users_UserId",
                table: "AssociationTenantPlayer",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchTeam_AssociationTeam_MatchTeamId",
                table: "MatchTeam",
                column: "MatchTeamId",
                principalTable: "AssociationTeam",
                principalColumn: "AssociationTeamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
