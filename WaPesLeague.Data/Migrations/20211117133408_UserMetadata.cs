using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class UserMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessStatus",
                table: "FileImports",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "UnProcessed");

            migrationBuilder.CreateTable(
                name: "Metadatas",
                columns: table => new
                {
                    MetadataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "String")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadatas", x => x.MetadataId);
                });

            migrationBuilder.CreateTable(
                name: "ServerButtonGroups",
                columns: table => new
                {
                    ServerButtonGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    ButtonGroupType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "ShowAllAtTheSameTime"),
                    UseRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerButtonGroups", x => x.ServerButtonGroupId);
                    table.ForeignKey(
                        name: "FK_ServerButtonGroups_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMetadatas",
                columns: table => new
                {
                    UserMetadataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MetadataId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMetadatas", x => x.UserMetadataId);
                    table.ForeignKey(
                        name: "FK_UserMetadatas_Metadatas_MetadataId",
                        column: x => x.MetadataId,
                        principalTable: "Metadatas",
                        principalColumn: "MetadataId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMetadatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerButtons",
                columns: table => new
                {
                    ServerButtonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerButtonGroupId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    URL = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ShowFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShowUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerButtons", x => x.ServerButtonId);
                    table.ForeignKey(
                        name: "FK_ServerButtons_ServerButtonGroups_ServerButtonGroupId",
                        column: x => x.ServerButtonGroupId,
                        principalTable: "ServerButtonGroups",
                        principalColumn: "ServerButtonGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "MetadataId", "Code", "Description" },
                values: new object[,]
                {
                    { 1, "DiscordId", "The Discord Id of the entity" },
                    { 2, "WaPesDiscordName", "The Discord name used to register onto WaPes" },
                    { 3, "WaPesPSNName", "The PSN name used to register onto WaPes" }
                });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "MetadataId", "Code", "Description", "PropertyType" },
                values: new object[] { 4, "SpeakEnglish", "can speak english prop on registration", "Bool" });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "MetadataId", "Code", "Description" },
                values: new object[,]
                {
                    { 5, "FavouritePosition1", "Favourite Position 1 prop on registration" },
                    { 6, "FavouritePosition2", "Favourite Position 2 prop on registration" },
                    { 7, "Motto", "Football Motto" },
                    { 8, "FootballStyle", "Your FootballStyle" },
                    { 9, "Quality1", "Football Quality 1" },
                    { 10, "Quality2", "Football Quality 2" },
                    { 11, "Quality3", "Football Quality 3" }
                });

            migrationBuilder.InsertData(
                table: "Metadatas",
                columns: new[] { "MetadataId", "Code", "Description", "PropertyType" },
                values: new object[] { 12, "WaPesJoinDate", "WaPes Joining Date", "DateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ServerButtonGroups_ServerId",
                table: "ServerButtonGroups",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerButtons_ServerButtonGroupId",
                table: "ServerButtons",
                column: "ServerButtonGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetadatas_MetadataId",
                table: "UserMetadatas",
                column: "MetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetadatas_UserId",
                table: "UserMetadatas",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerButtons");

            migrationBuilder.DropTable(
                name: "UserMetadatas");

            migrationBuilder.DropTable(
                name: "ServerButtonGroups");

            migrationBuilder.DropTable(
                name: "Metadatas");

            migrationBuilder.DropColumn(
                name: "ProcessStatus",
                table: "FileImports");
        }
    }
}
