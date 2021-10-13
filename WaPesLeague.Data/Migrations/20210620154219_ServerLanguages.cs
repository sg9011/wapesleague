using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class ServerLanguages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileImport_FileImportType_FileImportTypeId",
                table: "FileImport");

            migrationBuilder.DropTable(
                name: "FileImportType");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "FileImport");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Servers",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "EN");

            migrationBuilder.CreateTable(
                name: "GoogleSheetImportType",
                columns: table => new
                {
                    GoogleSheetImportTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartRow = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GoogleSheetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TabName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GoogleSheetId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleSheetImportType", x => x.GoogleSheetImportTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FileImport_GoogleSheetImportType_FileImportTypeId",
                table: "FileImport",
                column: "FileImportTypeId",
                principalTable: "GoogleSheetImportType",
                principalColumn: "GoogleSheetImportTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileImport_GoogleSheetImportType_FileImportTypeId",
                table: "FileImport");

            migrationBuilder.DropTable(
                name: "GoogleSheetImportType");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Servers");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "FileImport",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FileImportType",
                columns: table => new
                {
                    FileImportTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartRow = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileImportType", x => x.FileImportTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FileImport_FileImportType_FileImportTypeId",
                table: "FileImport",
                column: "FileImportTypeId",
                principalTable: "FileImportType",
                principalColumn: "FileImportTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
