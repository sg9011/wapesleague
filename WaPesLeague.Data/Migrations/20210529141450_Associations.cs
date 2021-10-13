using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class Associations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssociationTenant",
                columns: table => new
                {
                    AssociationTenantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationTenant", x => x.AssociationTenantId);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeamPlayerStatType",
                columns: table => new
                {
                    MatchTeamPlayerStatTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeamPlayerStatType", x => x.MatchTeamPlayerStatTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeamStatType",
                columns: table => new
                {
                    MatchTeamStatTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeamStatType", x => x.MatchTeamStatTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Association",
                columns: table => new
                {
                    AssociationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociationTenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Association", x => x.AssociationId);
                    table.ForeignKey(
                        name: "FK_Association_AssociationTenant_AssociationTenantId",
                        column: x => x.AssociationTenantId,
                        principalTable: "AssociationTenant",
                        principalColumn: "AssociationTenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationTenantPlayer",
                columns: table => new
                {
                    AssociationTenantPlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssociationTenantId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationTenantPlayer", x => x.AssociationTenantPlayerId);
                    table.ForeignKey(
                        name: "FK_AssociationTenantPlayer_AssociationTenant_AssociationTenantId",
                        column: x => x.AssociationTenantId,
                        principalTable: "AssociationTenant",
                        principalColumn: "AssociationTenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationTenantPlayer_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationLeagueGroup",
                columns: table => new
                {
                    AssociationLeagueGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationLeagueGroup", x => x.AssociationLeagueGroupId);
                    table.ForeignKey(
                        name: "FK_AssociationLeagueGroup_Association_AssociationId",
                        column: x => x.AssociationId,
                        principalTable: "Association",
                        principalColumn: "AssociationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationTeam",
                columns: table => new
                {
                    AssociationTeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociationId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationTeam", x => x.AssociationTeamId);
                    table.ForeignKey(
                        name: "FK_AssociationTeam_Association_AssociationId",
                        column: x => x.AssociationId,
                        principalTable: "Association",
                        principalColumn: "AssociationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationLeagueSeason",
                columns: table => new
                {
                    AssociationLeagueSeasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociationLeagueGroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Edition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationLeagueSeason", x => x.AssociationLeagueSeasonId);
                    table.ForeignKey(
                        name: "FK_AssociationLeagueSeason_AssociationLeagueGroup_AssociationLeagueGroupId",
                        column: x => x.AssociationLeagueGroupId,
                        principalTable: "AssociationLeagueGroup",
                        principalColumn: "AssociationLeagueGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationTeamPlayer",
                columns: table => new
                {
                    AssociationTeamPlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociationTeamId = table.Column<int>(type: "int", nullable: false),
                    AssociationTenantPlayerId = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TeamMemberType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationTeamPlayer", x => x.AssociationTeamPlayerId);
                    table.ForeignKey(
                        name: "FK_AssociationTeamPlayer_AssociationTeam_AssociationTeamId",
                        column: x => x.AssociationTeamId,
                        principalTable: "AssociationTeam",
                        principalColumn: "AssociationTeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationTeamPlayer_AssociationTenantPlayer_AssociationTenantPlayerId",
                        column: x => x.AssociationTenantPlayerId,
                        principalTable: "AssociationTenantPlayer",
                        principalColumn: "AssociationTenantPlayerId");
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    DivisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociationLeagueSeasonId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.DivisionId);
                    table.ForeignKey(
                        name: "FK_Division_AssociationLeagueSeason_AssociationLeagueSeasonId",
                        column: x => x.AssociationLeagueSeasonId,
                        principalTable: "AssociationLeagueSeason",
                        principalColumn: "AssociationLeagueSeasonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DivisionRound",
                columns: table => new
                {
                    DivisionRoundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivisionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionRound", x => x.DivisionRoundId);
                    table.ForeignKey(
                        name: "FK_DivisionRound_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DivisionGroup",
                columns: table => new
                {
                    DivisionGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivisionRoundId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionGroup", x => x.DivisionGroupId);
                    table.ForeignKey(
                        name: "FK_DivisionGroup_DivisionRound_DivisionRoundId",
                        column: x => x.DivisionRoundId,
                        principalTable: "DivisionRound",
                        principalColumn: "DivisionRoundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DivisionGroupRound",
                columns: table => new
                {
                    DivisionGroupRoundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivisionGroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionGroupRound", x => x.DivisionGroupRoundId);
                    table.ForeignKey(
                        name: "FK_DivisionGroupRound_DivisionGroup_DivisionGroupId",
                        column: x => x.DivisionGroupId,
                        principalTable: "DivisionGroup",
                        principalColumn: "DivisionGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    MatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionGroupRoundId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatePlanned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatePlayed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MatchStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Match_DivisionGroupRound_DivisionGroupRoundId",
                        column: x => x.DivisionGroupRoundId,
                        principalTable: "DivisionGroupRound",
                        principalColumn: "DivisionGroupRoundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeam",
                columns: table => new
                {
                    MatchTeamId = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Goals = table.Column<int>(type: "int", nullable: true),
                    DateConfirmed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeam", x => x.MatchTeamId);
                    table.ForeignKey(
                        name: "FK_MatchTeam_AssociationTeam_MatchTeamId",
                        column: x => x.MatchTeamId,
                        principalTable: "AssociationTeam",
                        principalColumn: "AssociationTeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchTeam_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId");
                    table.ForeignKey(
                        name: "FK_MatchTeam_Users_ConfirmedById",
                        column: x => x.ConfirmedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeamPlayer",
                columns: table => new
                {
                    MatchTeamPlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchTeamId = table.Column<int>(type: "int", nullable: false),
                    AssociationTeamPlayerId = table.Column<int>(type: "int", nullable: true),
                    PlayerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    JerseyNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeamPlayer", x => x.MatchTeamPlayerId);
                    table.ForeignKey(
                        name: "FK_MatchTeamPlayer_AssociationTeamPlayer_AssociationTeamPlayerId",
                        column: x => x.AssociationTeamPlayerId,
                        principalTable: "AssociationTeamPlayer",
                        principalColumn: "AssociationTeamPlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchTeamPlayer_MatchTeam_MatchTeamId",
                        column: x => x.MatchTeamId,
                        principalTable: "MatchTeam",
                        principalColumn: "MatchTeamId");
                    table.ForeignKey(
                        name: "FK_MatchTeamPlayer_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeamStat",
                columns: table => new
                {
                    MatchTeamStatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchTeamId = table.Column<int>(type: "int", nullable: false),
                    MatchTeamStatTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeamStat", x => x.MatchTeamStatId);
                    table.ForeignKey(
                        name: "FK_MatchTeamStat_MatchTeam_MatchTeamId",
                        column: x => x.MatchTeamId,
                        principalTable: "MatchTeam",
                        principalColumn: "MatchTeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchTeamStat_MatchTeamStatType_MatchTeamStatTypeId",
                        column: x => x.MatchTeamStatTypeId,
                        principalTable: "MatchTeamStatType",
                        principalColumn: "MatchTeamStatTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeamPlayerEvent",
                columns: table => new
                {
                    MatchTeamPlayerEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchTeamPlayerId = table.Column<int>(type: "int", nullable: false),
                    Event = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: true),
                    Period = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeamPlayerEvent", x => x.MatchTeamPlayerEventId);
                    table.ForeignKey(
                        name: "FK_MatchTeamPlayerEvent_MatchTeamPlayer_MatchTeamPlayerId",
                        column: x => x.MatchTeamPlayerId,
                        principalTable: "MatchTeamPlayer",
                        principalColumn: "MatchTeamPlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeamPlayerStat",
                columns: table => new
                {
                    MatchTeamPlayerStatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchTeamPlayerId = table.Column<int>(type: "int", nullable: false),
                    MatchPlayerStatTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeamPlayerStat", x => x.MatchTeamPlayerStatId);
                    table.ForeignKey(
                        name: "FK_MatchTeamPlayerStat_MatchTeamPlayer_MatchTeamPlayerId",
                        column: x => x.MatchTeamPlayerId,
                        principalTable: "MatchTeamPlayer",
                        principalColumn: "MatchTeamPlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchTeamPlayerStat_MatchTeamPlayerStatType_MatchPlayerStatTypeId",
                        column: x => x.MatchPlayerStatTypeId,
                        principalTable: "MatchTeamPlayerStatType",
                        principalColumn: "MatchTeamPlayerStatTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MatchTeamPlayerStatType",
                columns: new[] { "MatchTeamPlayerStatTypeId", "Code", "Description", "Order" },
                values: new object[,]
                {
                    { 1, "InGameRating", "In Game Rating", 1 },
                    { 17, "Offsides", "Offsides", 903 },
                    { 16, "Fouls", "Fouls", 902 },
                    { 15, "Touches", "Touches", 901 },
                    { 14, "Interceptions", "Interceptions", 900 },
                    { 12, "TacklesCompleted", "Tackles completed", 301 },
                    { 11, "Tackles", "Tackles attempted", 300 },
                    { 10, "PassPerc", "Passing percentage", 202 },
                    { 13, "TacklesPerc", "Tackles percentage", 302 },
                    { 8, "Passes", "Passes attempted", 200 },
                    { 7, "ShotsOnTargetPerc", "Shots on target percentage", 102 },
                    { 6, "ShotsOnTarget", "Shots on target", 101 },
                    { 5, "Shots", "Shots", 100 },
                    { 4, "Assists", "Assists", 20 },
                    { 3, "OwnGoals", "Own goals", 10 },
                    { 2, "Goals", "Goals", 10 },
                    { 9, "PassesCompleted", "Passes completed", 201 }
                });

            migrationBuilder.InsertData(
                table: "MatchTeamStatType",
                columns: new[] { "MatchTeamStatTypeId", "Code", "Description", "Order" },
                values: new object[,]
                {
                    { 11, "Passes", "Passes attempted", 300 },
                    { 17, "Interceptions", "Interceptions", 500 },
                    { 16, "TacklesPerc", "Tackles percentage", 402 },
                    { 15, "TacklesCompleted", "Tackles completed", 401 },
                    { 14, "Tackles", "Tackles attempted", 400 },
                    { 13, "PassPerc", "Passing percentage", 302 },
                    { 12, "PassesCompleted", "Passes completed", 301 },
                    { 10, "ShotsOnTargetPerc", "Shots on target percentage", 202 },
                    { 1, "Pos", "Possession", 100 },
                    { 8, "Shots", "Shots", 200 },
                    { 7, "RightPos", "Possession on the right side", 122 },
                    { 6, "CenterPos", "Possession in the center of the pitch", 121 },
                    { 5, "LeftPos", "Possession on the left side", 120 },
                    { 4, "AttPos", "Possession in final third", 112 },
                    { 3, "MidPos", "Possession in midfield third", 111 },
                    { 2, "DefPos", "Possession in defensive third", 110 },
                    { 18, "Fouls", "Fouls committed", 600 },
                    { 9, "ShotsOnTarget", "Shots on target", 201 },
                    { 19, "Offides", "Offsides", 601 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Association_AssociationTenantId",
                table: "Association",
                column: "AssociationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationLeagueGroup_AssociationId",
                table: "AssociationLeagueGroup",
                column: "AssociationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationLeagueSeason_AssociationLeagueGroupId",
                table: "AssociationLeagueSeason",
                column: "AssociationLeagueGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTeam_AssociationId",
                table: "AssociationTeam",
                column: "AssociationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTeamPlayer_AssociationTeamId",
                table: "AssociationTeamPlayer",
                column: "AssociationTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTeamPlayer_AssociationTenantPlayerId",
                table: "AssociationTeamPlayer",
                column: "AssociationTenantPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTenantPlayer_AssociationTenantId",
                table: "AssociationTenantPlayer",
                column: "AssociationTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTenantPlayer_UserId",
                table: "AssociationTenantPlayer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Division_AssociationLeagueSeasonId",
                table: "Division",
                column: "AssociationLeagueSeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_DivisionGroup_DivisionRoundId",
                table: "DivisionGroup",
                column: "DivisionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_DivisionGroupRound_DivisionGroupId",
                table: "DivisionGroupRound",
                column: "DivisionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DivisionRound_DivisionId",
                table: "DivisionRound",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_DivisionGroupRoundId",
                table: "Match",
                column: "DivisionGroupRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeam_ConfirmedById",
                table: "MatchTeam",
                column: "ConfirmedById");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeam_MatchId",
                table: "MatchTeam",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamPlayer_AssociationTeamPlayerId",
                table: "MatchTeamPlayer",
                column: "AssociationTeamPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamPlayer_MatchTeamId",
                table: "MatchTeamPlayer",
                column: "MatchTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamPlayer_PositionId",
                table: "MatchTeamPlayer",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamPlayerEvent_MatchTeamPlayerId",
                table: "MatchTeamPlayerEvent",
                column: "MatchTeamPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamPlayerStat_MatchPlayerStatTypeId",
                table: "MatchTeamPlayerStat",
                column: "MatchPlayerStatTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamPlayerStat_MatchTeamPlayerId",
                table: "MatchTeamPlayerStat",
                column: "MatchTeamPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamStat_MatchTeamId",
                table: "MatchTeamStat",
                column: "MatchTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeamStat_MatchTeamStatTypeId",
                table: "MatchTeamStat",
                column: "MatchTeamStatTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchTeamPlayerEvent");

            migrationBuilder.DropTable(
                name: "MatchTeamPlayerStat");

            migrationBuilder.DropTable(
                name: "MatchTeamStat");

            migrationBuilder.DropTable(
                name: "MatchTeamPlayer");

            migrationBuilder.DropTable(
                name: "MatchTeamPlayerStatType");

            migrationBuilder.DropTable(
                name: "MatchTeamStatType");

            migrationBuilder.DropTable(
                name: "AssociationTeamPlayer");

            migrationBuilder.DropTable(
                name: "MatchTeam");

            migrationBuilder.DropTable(
                name: "AssociationTenantPlayer");

            migrationBuilder.DropTable(
                name: "AssociationTeam");

            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "DivisionGroupRound");

            migrationBuilder.DropTable(
                name: "DivisionGroup");

            migrationBuilder.DropTable(
                name: "DivisionRound");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "AssociationLeagueSeason");

            migrationBuilder.DropTable(
                name: "AssociationLeagueGroup");

            migrationBuilder.DropTable(
                name: "Association");

            migrationBuilder.DropTable(
                name: "AssociationTenant");
        }
    }
}
