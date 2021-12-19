using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class PluralTableNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Association_AssociationTenant_AssociationTenantId",
                table: "Association");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationLeagueGroup_Association_AssociationId",
                table: "AssociationLeagueGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationLeagueSeason_AssociationLeagueGroup_AssociationLeagueGroupId",
                table: "AssociationLeagueSeason");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeam_Association_AssociationId",
                table: "AssociationTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTenantPlayer_AssociationTenant_AssociationTenantId",
                table: "AssociationTenantPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_Division_AssociationLeagueSeason_AssociationLeagueSeasonId",
                table: "Division");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionGroup_DivisionRound_DivisionRoundId",
                table: "DivisionGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionGroupRound_DivisionGroup_DivisionGroupId",
                table: "DivisionGroupRound");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionRound_Division_DivisionId",
                table: "DivisionRound");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_DivisionGroupRound_DivisionGroupRoundId",
                table: "Match");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivisionRound",
                table: "DivisionRound");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivisionGroupRound",
                table: "DivisionGroupRound");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivisionGroup",
                table: "DivisionGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Division",
                table: "Division");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationTenant",
                table: "AssociationTenant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationLeagueSeason",
                table: "AssociationLeagueSeason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationLeagueGroup",
                table: "AssociationLeagueGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Association",
                table: "Association");

            migrationBuilder.RenameTable(
                name: "DivisionRound",
                newName: "DivisionRounds");

            migrationBuilder.RenameTable(
                name: "DivisionGroupRound",
                newName: "DivisionGroupRounds");

            migrationBuilder.RenameTable(
                name: "DivisionGroup",
                newName: "DivisionGroups");

            migrationBuilder.RenameTable(
                name: "Division",
                newName: "Divisions");

            migrationBuilder.RenameTable(
                name: "AssociationTenant",
                newName: "AssociationTenants");

            migrationBuilder.RenameTable(
                name: "AssociationLeagueSeason",
                newName: "AssociationLeagueSeasons");

            migrationBuilder.RenameTable(
                name: "AssociationLeagueGroup",
                newName: "AssociationLeagueGroups");

            migrationBuilder.RenameTable(
                name: "Association",
                newName: "Associations");

            migrationBuilder.RenameIndex(
                name: "IX_DivisionRound_DivisionId",
                table: "DivisionRounds",
                newName: "IX_DivisionRounds_DivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_DivisionGroupRound_DivisionGroupId",
                table: "DivisionGroupRounds",
                newName: "IX_DivisionGroupRounds_DivisionGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_DivisionGroup_DivisionRoundId",
                table: "DivisionGroups",
                newName: "IX_DivisionGroups_DivisionRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_Division_AssociationLeagueSeasonId",
                table: "Divisions",
                newName: "IX_Divisions_AssociationLeagueSeasonId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationLeagueSeason_AssociationLeagueGroupId",
                table: "AssociationLeagueSeasons",
                newName: "IX_AssociationLeagueSeasons_AssociationLeagueGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationLeagueGroup_AssociationId",
                table: "AssociationLeagueGroups",
                newName: "IX_AssociationLeagueGroups_AssociationId");

            migrationBuilder.RenameIndex(
                name: "IX_Association_AssociationTenantId",
                table: "Associations",
                newName: "IX_Associations_AssociationTenantId");

            migrationBuilder.AddColumn<int>(
                name: "GoogleSheetImportTypeId",
                table: "DivisionGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivisionRounds",
                table: "DivisionRounds",
                column: "DivisionRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivisionGroupRounds",
                table: "DivisionGroupRounds",
                column: "DivisionGroupRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivisionGroups",
                table: "DivisionGroups",
                column: "DivisionGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions",
                column: "DivisionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationTenants",
                table: "AssociationTenants",
                column: "AssociationTenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationLeagueSeasons",
                table: "AssociationLeagueSeasons",
                column: "AssociationLeagueSeasonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationLeagueGroups",
                table: "AssociationLeagueGroups",
                column: "AssociationLeagueGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Associations",
                table: "Associations",
                column: "AssociationId");

            migrationBuilder.CreateIndex(
                name: "IX_DivisionGroups_GoogleSheetImportTypeId",
                table: "DivisionGroups",
                column: "GoogleSheetImportTypeId",
                unique: true,
                filter: "[GoogleSheetImportTypeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationLeagueGroups_Associations_AssociationId",
                table: "AssociationLeagueGroups",
                column: "AssociationId",
                principalTable: "Associations",
                principalColumn: "AssociationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationLeagueSeasons_AssociationLeagueGroups_AssociationLeagueGroupId",
                table: "AssociationLeagueSeasons",
                column: "AssociationLeagueGroupId",
                principalTable: "AssociationLeagueGroups",
                principalColumn: "AssociationLeagueGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Associations_AssociationTenants_AssociationTenantId",
                table: "Associations",
                column: "AssociationTenantId",
                principalTable: "AssociationTenants",
                principalColumn: "AssociationTenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeam_Associations_AssociationId",
                table: "AssociationTeam",
                column: "AssociationId",
                principalTable: "Associations",
                principalColumn: "AssociationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTenantPlayer_AssociationTenants_AssociationTenantId",
                table: "AssociationTenantPlayer",
                column: "AssociationTenantId",
                principalTable: "AssociationTenants",
                principalColumn: "AssociationTenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionGroupRounds_DivisionGroups_DivisionGroupId",
                table: "DivisionGroupRounds",
                column: "DivisionGroupId",
                principalTable: "DivisionGroups",
                principalColumn: "DivisionGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionGroups_DivisionRounds_DivisionRoundId",
                table: "DivisionGroups",
                column: "DivisionRoundId",
                principalTable: "DivisionRounds",
                principalColumn: "DivisionRoundId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionGroups_GoogleSheetImportTypes_GoogleSheetImportTypeId",
                table: "DivisionGroups",
                column: "GoogleSheetImportTypeId",
                principalTable: "GoogleSheetImportTypes",
                principalColumn: "GoogleSheetImportTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionRounds_Divisions_DivisionId",
                table: "DivisionRounds",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Divisions_AssociationLeagueSeasons_AssociationLeagueSeasonId",
                table: "Divisions",
                column: "AssociationLeagueSeasonId",
                principalTable: "AssociationLeagueSeasons",
                principalColumn: "AssociationLeagueSeasonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_DivisionGroupRounds_DivisionGroupRoundId",
                table: "Match",
                column: "DivisionGroupRoundId",
                principalTable: "DivisionGroupRounds",
                principalColumn: "DivisionGroupRoundId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssociationLeagueGroups_Associations_AssociationId",
                table: "AssociationLeagueGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationLeagueSeasons_AssociationLeagueGroups_AssociationLeagueGroupId",
                table: "AssociationLeagueSeasons");

            migrationBuilder.DropForeignKey(
                name: "FK_Associations_AssociationTenants_AssociationTenantId",
                table: "Associations");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTeam_Associations_AssociationId",
                table: "AssociationTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationTenantPlayer_AssociationTenants_AssociationTenantId",
                table: "AssociationTenantPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionGroupRounds_DivisionGroups_DivisionGroupId",
                table: "DivisionGroupRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionGroups_DivisionRounds_DivisionRoundId",
                table: "DivisionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionGroups_GoogleSheetImportTypes_GoogleSheetImportTypeId",
                table: "DivisionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionRounds_Divisions_DivisionId",
                table: "DivisionRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_AssociationLeagueSeasons_AssociationLeagueSeasonId",
                table: "Divisions");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_DivisionGroupRounds_DivisionGroupRoundId",
                table: "Match");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Divisions",
                table: "Divisions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivisionRounds",
                table: "DivisionRounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivisionGroups",
                table: "DivisionGroups");

            migrationBuilder.DropIndex(
                name: "IX_DivisionGroups_GoogleSheetImportTypeId",
                table: "DivisionGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivisionGroupRounds",
                table: "DivisionGroupRounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationTenants",
                table: "AssociationTenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Associations",
                table: "Associations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationLeagueSeasons",
                table: "AssociationLeagueSeasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssociationLeagueGroups",
                table: "AssociationLeagueGroups");

            migrationBuilder.DropColumn(
                name: "GoogleSheetImportTypeId",
                table: "DivisionGroups");

            migrationBuilder.RenameTable(
                name: "Divisions",
                newName: "Division");

            migrationBuilder.RenameTable(
                name: "DivisionRounds",
                newName: "DivisionRound");

            migrationBuilder.RenameTable(
                name: "DivisionGroups",
                newName: "DivisionGroup");

            migrationBuilder.RenameTable(
                name: "DivisionGroupRounds",
                newName: "DivisionGroupRound");

            migrationBuilder.RenameTable(
                name: "AssociationTenants",
                newName: "AssociationTenant");

            migrationBuilder.RenameTable(
                name: "Associations",
                newName: "Association");

            migrationBuilder.RenameTable(
                name: "AssociationLeagueSeasons",
                newName: "AssociationLeagueSeason");

            migrationBuilder.RenameTable(
                name: "AssociationLeagueGroups",
                newName: "AssociationLeagueGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Divisions_AssociationLeagueSeasonId",
                table: "Division",
                newName: "IX_Division_AssociationLeagueSeasonId");

            migrationBuilder.RenameIndex(
                name: "IX_DivisionRounds_DivisionId",
                table: "DivisionRound",
                newName: "IX_DivisionRound_DivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_DivisionGroups_DivisionRoundId",
                table: "DivisionGroup",
                newName: "IX_DivisionGroup_DivisionRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_DivisionGroupRounds_DivisionGroupId",
                table: "DivisionGroupRound",
                newName: "IX_DivisionGroupRound_DivisionGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Associations_AssociationTenantId",
                table: "Association",
                newName: "IX_Association_AssociationTenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationLeagueSeasons_AssociationLeagueGroupId",
                table: "AssociationLeagueSeason",
                newName: "IX_AssociationLeagueSeason_AssociationLeagueGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_AssociationLeagueGroups_AssociationId",
                table: "AssociationLeagueGroup",
                newName: "IX_AssociationLeagueGroup_AssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Division",
                table: "Division",
                column: "DivisionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivisionRound",
                table: "DivisionRound",
                column: "DivisionRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivisionGroup",
                table: "DivisionGroup",
                column: "DivisionGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivisionGroupRound",
                table: "DivisionGroupRound",
                column: "DivisionGroupRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationTenant",
                table: "AssociationTenant",
                column: "AssociationTenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Association",
                table: "Association",
                column: "AssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationLeagueSeason",
                table: "AssociationLeagueSeason",
                column: "AssociationLeagueSeasonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssociationLeagueGroup",
                table: "AssociationLeagueGroup",
                column: "AssociationLeagueGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Association_AssociationTenant_AssociationTenantId",
                table: "Association",
                column: "AssociationTenantId",
                principalTable: "AssociationTenant",
                principalColumn: "AssociationTenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationLeagueGroup_Association_AssociationId",
                table: "AssociationLeagueGroup",
                column: "AssociationId",
                principalTable: "Association",
                principalColumn: "AssociationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationLeagueSeason_AssociationLeagueGroup_AssociationLeagueGroupId",
                table: "AssociationLeagueSeason",
                column: "AssociationLeagueGroupId",
                principalTable: "AssociationLeagueGroup",
                principalColumn: "AssociationLeagueGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTeam_Association_AssociationId",
                table: "AssociationTeam",
                column: "AssociationId",
                principalTable: "Association",
                principalColumn: "AssociationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationTenantPlayer_AssociationTenant_AssociationTenantId",
                table: "AssociationTenantPlayer",
                column: "AssociationTenantId",
                principalTable: "AssociationTenant",
                principalColumn: "AssociationTenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Division_AssociationLeagueSeason_AssociationLeagueSeasonId",
                table: "Division",
                column: "AssociationLeagueSeasonId",
                principalTable: "AssociationLeagueSeason",
                principalColumn: "AssociationLeagueSeasonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionGroup_DivisionRound_DivisionRoundId",
                table: "DivisionGroup",
                column: "DivisionRoundId",
                principalTable: "DivisionRound",
                principalColumn: "DivisionRoundId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionGroupRound_DivisionGroup_DivisionGroupId",
                table: "DivisionGroupRound",
                column: "DivisionGroupId",
                principalTable: "DivisionGroup",
                principalColumn: "DivisionGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionRound_Division_DivisionId",
                table: "DivisionRound",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_DivisionGroupRound_DivisionGroupRoundId",
                table: "Match",
                column: "DivisionGroupRoundId",
                principalTable: "DivisionGroupRound",
                principalColumn: "DivisionGroupRoundId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
