using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formations",
                columns: table => new
                {
                    FormationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formations", x => x.FormationId);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    PlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.PlatformId);
                });

            migrationBuilder.CreateTable(
                name: "PositionGroups",
                columns: table => new
                {
                    PositionGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    BaseValue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionGroups", x => x.PositionGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ServerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscordServerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultSessionRecurringWithAClosedTeam = table.Column<bool>(type: "bit", nullable: false),
                    DefaultSessionRecurringWithAllOpen = table.Column<bool>(type: "bit", nullable: false),
                    DefaultAutoCreateExtraSessionsWhenAllTeamsOpen = table.Column<bool>(type: "bit", nullable: false),
                    DefaultAutoCreateExtraSessionsWithAClosedTeam = table.Column<bool>(type: "bit", nullable: false),
                    DefaultStartTime = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DefaultHoursToOpenRegistrationBeforeStart = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DefaultSessionDuration = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DefaultSessionExtraInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultSessionPassword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UsePasswordForSessions = table.Column<bool>(type: "bit", nullable: false),
                    UseServerForSessions = table.Column<bool>(type: "bit", nullable: false),
                    ShowPESSideSelectionInfo = table.Column<bool>(type: "bit", nullable: false),
                    TimeZoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ServerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExtraInfo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "FormationTags",
                columns: table => new
                {
                    FormationTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormationId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormationTags", x => x.FormationTagId);
                    table.ForeignKey(
                        name: "FK_FormationTags_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "FormationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionGroupId = table.Column<int>(type: "int", nullable: false),
                    ParentPositionId = table.Column<int>(type: "int", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsRequiredForMix = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_Positions_PositionGroups_PositionGroupId",
                        column: x => x.PositionGroupId,
                        principalTable: "PositionGroups",
                        principalColumn: "PositionGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Positions_Positions_ParentPositionId",
                        column: x => x.ParentPositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MixGroups",
                columns: table => new
                {
                    MixGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    Recurring = table.Column<bool>(type: "bit", nullable: false),
                    CreateExtraMixChannels = table.Column<bool>(type: "bit", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Start = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    HoursToOpenRegistrationBeforeStart = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaxSessionDurationInHours = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixGroups", x => x.MixGroupId);
                    table.ForeignKey(
                        name: "FK_MixGroups_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerFormations",
                columns: table => new
                {
                    ServerFormationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerFormations", x => x.ServerFormationId);
                    table.ForeignKey(
                        name: "FK_ServerFormations_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerTeams",
                columns: table => new
                {
                    ServerTeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerTeams", x => x.ServerTeamId);
                    table.ForeignKey(
                        name: "FK_ServerTeams_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMembers",
                columns: table => new
                {
                    UserMemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    DiscordUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiscordNickName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DiscordMention = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DiscordUserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMembers", x => x.UserMemberId);
                    table.ForeignKey(
                        name: "FK_UserMembers_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlatforms",
                columns: table => new
                {
                    UserPlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatforms", x => x.UserPlatformId);
                    table.ForeignKey(
                        name: "FK_UserPlatforms_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "PlatformId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPlatforms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormationPositions",
                columns: table => new
                {
                    FormationPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    FormationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormationPositions", x => x.FormationPositionId);
                    table.ForeignKey(
                        name: "FK_FormationPositions_Formations_FormationId",
                        column: x => x.FormationId,
                        principalTable: "Formations",
                        principalColumn: "FormationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormationPositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionTags",
                columns: table => new
                {
                    PositionTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionTags", x => x.PositionTagId);
                    table.ForeignKey(
                        name: "FK_PositionTags_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixChannels",
                columns: table => new
                {
                    MixChannelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixGroupId = table.Column<int>(type: "int", nullable: false),
                    DiscordChannelId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixChannels", x => x.MixChannelId);
                    table.ForeignKey(
                        name: "FK_MixChannels_MixGroups_MixGroupId",
                        column: x => x.MixGroupId,
                        principalTable: "MixGroups",
                        principalColumn: "MixGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerFormationPositions",
                columns: table => new
                {
                    ServerFormationPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    ServerFormationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerFormationPositions", x => x.ServerFormationPositionId);
                    table.ForeignKey(
                        name: "FK_ServerFormationPositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServerFormationPositions_ServerFormations_ServerFormationId",
                        column: x => x.ServerFormationId,
                        principalTable: "ServerFormations",
                        principalColumn: "ServerFormationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerFormationTags",
                columns: table => new
                {
                    ServerFormationTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerFormationId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerFormationTags", x => x.ServerFormationTagId);
                    table.ForeignKey(
                        name: "FK_ServerFormationTags_ServerFormations_ServerFormationId",
                        column: x => x.ServerFormationId,
                        principalTable: "ServerFormations",
                        principalColumn: "ServerFormationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerTeamTags",
                columns: table => new
                {
                    ServerTeamTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerTeamId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerTeamTags", x => x.ServerTeamTagId);
                    table.ForeignKey(
                        name: "FK_ServerTeamTags_ServerTeams_ServerTeamId",
                        column: x => x.ServerTeamId,
                        principalTable: "ServerTeams",
                        principalColumn: "ServerTeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixChannelTeams",
                columns: table => new
                {
                    MixChannelTeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixChannelId = table.Column<int>(type: "int", nullable: false),
                    MixChannelTeamName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixChannelTeams", x => x.MixChannelTeamId);
                    table.ForeignKey(
                        name: "FK_MixChannelTeams_MixChannels_MixChannelId",
                        column: x => x.MixChannelId,
                        principalTable: "MixChannels",
                        principalColumn: "MixChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixSessions",
                columns: table => new
                {
                    MixSessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixChannelId = table.Column<int>(type: "int", nullable: false),
                    DateRegistrationOpening = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateToClose = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CrashCount = table.Column<int>(type: "int", nullable: false),
                    MatchCount = table.Column<int>(type: "int", nullable: false),
                    DateLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GameRoomName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoomOwnerId = table.Column<int>(type: "int", nullable: true),
                    Server = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FollowUpSessionActivated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixSessions", x => x.MixSessionId);
                    table.ForeignKey(
                        name: "FK_MixSessions_MixChannels_MixChannelId",
                        column: x => x.MixChannelId,
                        principalTable: "MixChannels",
                        principalColumn: "MixChannelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MixSessions_Users_RoomOwnerId",
                        column: x => x.RoomOwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MixChannelTeamPositions",
                columns: table => new
                {
                    MixChannelTeamPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixChannelTeamId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixChannelTeamPositions", x => x.MixChannelTeamPositionId);
                    table.ForeignKey(
                        name: "FK_MixChannelTeamPositions_MixChannelTeams_MixChannelTeamId",
                        column: x => x.MixChannelTeamId,
                        principalTable: "MixChannelTeams",
                        principalColumn: "MixChannelTeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MixChannelTeamPositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixChannelTeamTags",
                columns: table => new
                {
                    MixChannelTeamTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixChannelTeamId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixChannelTeamTags", x => x.MixChannelTeamTagId);
                    table.ForeignKey(
                        name: "FK_MixChannelTeamTags_MixChannelTeams_MixChannelTeamId",
                        column: x => x.MixChannelTeamId,
                        principalTable: "MixChannelTeams",
                        principalColumn: "MixChannelTeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixTeams",
                columns: table => new
                {
                    MixTeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixSessionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PositionsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedTeamPlayerCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixTeams", x => x.MixTeamId);
                    table.ForeignKey(
                        name: "FK_MixTeams_MixSessions_MixSessionId",
                        column: x => x.MixSessionId,
                        principalTable: "MixSessions",
                        principalColumn: "MixSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixPositions",
                columns: table => new
                {
                    MixPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixTeamId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixPositions", x => x.MixPositionId);
                    table.ForeignKey(
                        name: "FK_MixPositions_MixTeams_MixTeamId",
                        column: x => x.MixTeamId,
                        principalTable: "MixTeams",
                        principalColumn: "MixTeamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MixPositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixTeamTags",
                columns: table => new
                {
                    MixTeamTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixTeamId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixTeamTags", x => x.MixTeamTagId);
                    table.ForeignKey(
                        name: "FK_MixTeamTags_MixTeams_MixTeamId",
                        column: x => x.MixTeamId,
                        principalTable: "MixTeams",
                        principalColumn: "MixTeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixPositionReservations",
                columns: table => new
                {
                    MixPositionReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixPositionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCaptain = table.Column<bool>(type: "bit", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixPositionReservations", x => x.MixPositionReservationId);
                    table.ForeignKey(
                        name: "FK_MixPositionReservations_MixPositions_MixPositionId",
                        column: x => x.MixPositionId,
                        principalTable: "MixPositions",
                        principalColumn: "MixPositionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MixPositionReservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Formations",
                columns: new[] { "FormationId", "IsDefault", "Name" },
                values: new object[,]
                {
                    { 1, true, "Basic_433" },
                    { 2, false, "Basic_442" }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "PlatformId", "Description", "Name" },
                values: new object[] { 1, "The Playstation Network Platform", "PSN" });

            migrationBuilder.InsertData(
                table: "PositionGroups",
                columns: new[] { "PositionGroupId", "BaseValue", "Code", "Description", "Name", "Order" },
                values: new object[,]
                {
                    { 1, null, "GK", "Container for the goalkeeper position", "Goalkeeper", 1 },
                    { 2, null, "DEF", "Container for the defensive positions", "Defenders", 2 },
                    { 3, null, "MID", "Container for the midfield positions", "Midfielders", 3 },
                    { 4, null, "ATT", "Container for the attacking positions", "Attackers", 4 }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "PositionId", "Code", "Description", "IsRequiredForMix", "Order", "ParentPositionId", "PositionGroupId" },
                values: new object[,]
                {
                    { 1, "GK", "Goalkeeper", false, 1, null, 1 },
                    { 2, "LB", "Left Back", true, 1, null, 2 },
                    { 3, "LWB", "Left Wing Back", true, 2, null, 2 },
                    { 4, "CB", "Centre Back", true, 4, null, 2 },
                    { 7, "RB", "Right Back", true, 6, null, 2 },
                    { 8, "RWB", "Right Wing Back", true, 7, null, 2 },
                    { 9, "LM", "Left Midfielder", true, 1, null, 3 },
                    { 10, "DMF", "Central Defensive Midfielder", true, 3, null, 3 },
                    { 13, "CM", "Central Midfielder", true, 6, null, 3 },
                    { 16, "AMF", "Central Attacking Midfielder", true, 9, null, 3 },
                    { 19, "RM", "Right Midfielder", true, 11, null, 3 },
                    { 20, "LWF", "Left Wing Forward", true, 1, null, 4 },
                    { 21, "SS", "Second Striker", true, 3, null, 4 },
                    { 24, "CF", "Central Forward", true, 6, null, 4 },
                    { 27, "RWF", "Right Wing Forward", true, 8, null, 4 }
                });

            migrationBuilder.InsertData(
                table: "FormationPositions",
                columns: new[] { "FormationPositionId", "FormationId", "PositionId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 7, 1, 13 },
                    { 20, 2, 19 },
                    { 9, 1, 20 },
                    { 6, 1, 10 },
                    { 17, 2, 9 },
                    { 16, 2, 7 },
                    { 5, 1, 7 },
                    { 11, 1, 24 },
                    { 21, 2, 24 },
                    { 8, 1, 16 },
                    { 10, 1, 27 },
                    { 12, 2, 1 },
                    { 13, 2, 2 },
                    { 2, 1, 2 },
                    { 22, 2, 24 }
                });

            migrationBuilder.InsertData(
                table: "PositionTags",
                columns: new[] { "PositionTagId", "PositionId", "Tag" },
                values: new object[,]
                {
                    { 32, 16, "AM" },
                    { 33, 16, "AMC" },
                    { 34, 16, "MO" },
                    { 35, 16, "MOC" },
                    { 36, 16, "10" },
                    { 37, 16, "Riquelme" },
                    { 38, 16, "Zidane" },
                    { 43, 27, "RF" },
                    { 18, 19, "MD" },
                    { 49, 24, "9" },
                    { 39, 20, "LF" },
                    { 52, 24, "SP" },
                    { 40, 20, "Lang" },
                    { 41, 20, "ATG" },
                    { 31, 16, "CAM" },
                    { 46, 21, "F9" },
                    { 51, 24, "ST" },
                    { 47, 21, "FALSE9" },
                    { 48, 21, "FALSE 9" },
                    { 50, 24, "BU" },
                    { 19, 19, "RW" },
                    { 42, 20, "7" },
                    { 29, 13, "CMF" },
                    { 44, 27, "ATD" },
                    { 1, 1, "TW" },
                    { 2, 1, "G" }
                });

            migrationBuilder.InsertData(
                table: "PositionTags",
                columns: new[] { "PositionTagId", "PositionId", "Tag" },
                values: new object[,]
                {
                    { 3, 1, "1" },
                    { 6, 2, "DG" },
                    { 7, 2, "2" },
                    { 8, 4, "DC" },
                    { 9, 4, "CV" },
                    { 10, 4, "CH" },
                    { 11, 4, "SW" },
                    { 4, 7, "DD" },
                    { 30, 13, "8" },
                    { 16, 9, "MG" },
                    { 17, 9, "LW" },
                    { 5, 7, "5" },
                    { 45, 27, "11" },
                    { 20, 10, "MDC" },
                    { 21, 10, "CDM" },
                    { 22, 10, "Pirlo" },
                    { 23, 10, "Vieira" },
                    { 24, 10, "Kante" },
                    { 25, 10, "DM" },
                    { 26, 10, "6" },
                    { 28, 13, "MR" },
                    { 27, 13, "MC" }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "PositionId", "Code", "Description", "IsRequiredForMix", "Order", "ParentPositionId", "PositionGroupId" },
                values: new object[,]
                {
                    { 17, "LCAM", "Left Central Attacking Midfielder", true, 8, 16, 3 },
                    { 18, "RCAM", "Right Central Attacking Midfielder", true, 10, 16, 3 },
                    { 15, "RCM", "Right Central Midfielder", true, 7, 13, 3 },
                    { 5, "LCB", "Left Centre Back", true, 3, 4, 2 },
                    { 14, "LCM", "Left Central Midfielder", true, 5, 13, 3 },
                    { 25, "LCF", "Left Central Forward", true, 5, 24, 4 },
                    { 6, "RCB", "Right Centre Back", true, 5, 4, 2 },
                    { 12, "RCDM", "Right Central Defensive Midfielder", true, 4, 10, 3 },
                    { 23, "RSS", "Right Second Striker", true, 4, 21, 4 },
                    { 22, "LSS", "Left Wing Forward", true, 2, 21, 4 },
                    { 26, "RCF", "Right Central Striker", true, 7, 24, 4 },
                    { 11, "LCDM", "Left Central Defensive Midfielder", true, 2, 10, 3 }
                });

            migrationBuilder.InsertData(
                table: "FormationPositions",
                columns: new[] { "FormationPositionId", "FormationId", "PositionId" },
                values: new object[,]
                {
                    { 3, 1, 5 },
                    { 14, 2, 5 },
                    { 4, 1, 6 },
                    { 15, 2, 6 },
                    { 18, 2, 14 },
                    { 19, 2, 15 }
                });

            migrationBuilder.InsertData(
                table: "PositionTags",
                columns: new[] { "PositionTagId", "PositionId", "Tag" },
                values: new object[,]
                {
                    { 12, 5, "DCG" },
                    { 13, 5, "3" },
                    { 14, 6, "DCD" },
                    { 15, 6, "4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormationPositions_FormationId",
                table: "FormationPositions",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormationPositions_PositionId",
                table: "FormationPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormationTags_FormationId",
                table: "FormationTags",
                column: "FormationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormationTags_Tag",
                table: "FormationTags",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MixChannel_DiscordChannelId",
                table: "MixChannels",
                column: "DiscordChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MixChannels_MixGroupId",
                table: "MixChannels",
                column: "MixGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MixChannelTeamPositions_MixChannelTeamId",
                table: "MixChannelTeamPositions",
                column: "MixChannelTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MixChannelTeamPositions_PositionId",
                table: "MixChannelTeamPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_MixChannelTeams_MixChannelId",
                table: "MixChannelTeams",
                column: "MixChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MixChannelTeamTags_MixChannelTeamId",
                table: "MixChannelTeamTags",
                column: "MixChannelTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MixGroups_ServerId",
                table: "MixGroups",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_MixPositionReservations_MixPositionId",
                table: "MixPositionReservations",
                column: "MixPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_MixPositionReservations_UserId",
                table: "MixPositionReservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MixPositions_MixTeamId",
                table: "MixPositions",
                column: "MixTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MixPositions_PositionId",
                table: "MixPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_MixSessions_MixChannelId",
                table: "MixSessions",
                column: "MixChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MixSessions_RoomOwnerId",
                table: "MixSessions",
                column: "RoomOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MixTeams_MixSessionId",
                table: "MixTeams",
                column: "MixSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MixTeamTags_MixTeamId",
                table: "MixTeamTags",
                column: "MixTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionGroups_Order",
                table: "PositionGroups",
                column: "Order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ParentPositionId",
                table: "Positions",
                column: "ParentPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionGroupId_Order",
                table: "Positions",
                columns: new[] { "PositionGroupId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionTags_PositionId",
                table: "PositionTags",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerFormationPositions_PositionId",
                table: "ServerFormationPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerFormationPositions_ServerFormationId",
                table: "ServerFormationPositions",
                column: "ServerFormationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerFormations_ServerId",
                table: "ServerFormations",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerFormationTags_ServerFormationId",
                table: "ServerFormationTags",
                column: "ServerFormationId");

            migrationBuilder.CreateIndex(
                name: "IX_Server_DiscordServerId",
                table: "Servers",
                column: "DiscordServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerTeams_ServerId",
                table: "ServerTeams",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerTeamTags_ServerTeamId",
                table: "ServerTeamTags",
                column: "ServerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMembers_ServerId",
                table: "UserMembers",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMembers_UserId",
                table: "UserMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatforms_PlatformId",
                table: "UserPlatforms",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatforms_UserId",
                table: "UserPlatforms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Guid",
                table: "Users",
                column: "UserGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormationPositions");

            migrationBuilder.DropTable(
                name: "FormationTags");

            migrationBuilder.DropTable(
                name: "MixChannelTeamPositions");

            migrationBuilder.DropTable(
                name: "MixChannelTeamTags");

            migrationBuilder.DropTable(
                name: "MixPositionReservations");

            migrationBuilder.DropTable(
                name: "MixTeamTags");

            migrationBuilder.DropTable(
                name: "PositionTags");

            migrationBuilder.DropTable(
                name: "ServerFormationPositions");

            migrationBuilder.DropTable(
                name: "ServerFormationTags");

            migrationBuilder.DropTable(
                name: "ServerTeamTags");

            migrationBuilder.DropTable(
                name: "UserMembers");

            migrationBuilder.DropTable(
                name: "UserPlatforms");

            migrationBuilder.DropTable(
                name: "Formations");

            migrationBuilder.DropTable(
                name: "MixChannelTeams");

            migrationBuilder.DropTable(
                name: "MixPositions");

            migrationBuilder.DropTable(
                name: "ServerFormations");

            migrationBuilder.DropTable(
                name: "ServerTeams");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "MixTeams");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "MixSessions");

            migrationBuilder.DropTable(
                name: "PositionGroups");

            migrationBuilder.DropTable(
                name: "MixChannels");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MixGroups");

            migrationBuilder.DropTable(
                name: "Servers");
        }
    }
}
