using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class FileImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "FileImport",
                columns: table => new
                {
                    FileImportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileImportTypeId = table.Column<int>(type: "int", nullable: false),
                    FileStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileImport", x => x.FileImportId);
                    table.ForeignKey(
                        name: "FK_FileImport_FileImportType_FileImportTypeId",
                        column: x => x.FileImportTypeId,
                        principalTable: "FileImportType",
                        principalColumn: "FileImportTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileImportRecord",
                columns: table => new
                {
                    FileImportRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Record = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    FileImportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileImportRecord", x => x.FileImportRecordId);
                    table.ForeignKey(
                        name: "FK_FileImportRecord_FileImport_FileImportId",
                        column: x => x.FileImportId,
                        principalTable: "FileImport",
                        principalColumn: "FileImportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileImport_FileImportTypeId",
                table: "FileImport",
                column: "FileImportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileImportRecord_FileImportId",
                table: "FileImportRecord",
                column: "FileImportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileImportRecord");

            migrationBuilder.DropTable(
                name: "FileImport");

            migrationBuilder.DropTable(
                name: "FileImportType");
        }
    }
}
