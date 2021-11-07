using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class FileImportPluralNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileImport_GoogleSheetImportType_FileImportTypeId",
                table: "FileImport");

            migrationBuilder.DropForeignKey(
                name: "FK_FileImportRecord_FileImport_FileImportId",
                table: "FileImportRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoogleSheetImportType",
                table: "GoogleSheetImportType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileImportRecord",
                table: "FileImportRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileImport",
                table: "FileImport");

            migrationBuilder.RenameTable(
                name: "GoogleSheetImportType",
                newName: "GoogleSheetImportTypes");

            migrationBuilder.RenameTable(
                name: "FileImportRecord",
                newName: "FileImportRecords");

            migrationBuilder.RenameTable(
                name: "FileImport",
                newName: "FileImports");

            migrationBuilder.RenameIndex(
                name: "IX_FileImportRecord_FileImportId",
                table: "FileImportRecords",
                newName: "IX_FileImportRecords_FileImportId");

            migrationBuilder.RenameIndex(
                name: "IX_FileImport_FileImportTypeId",
                table: "FileImports",
                newName: "IX_FileImports_FileImportTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoogleSheetImportTypes",
                table: "GoogleSheetImportTypes",
                column: "GoogleSheetImportTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileImportRecords",
                table: "FileImportRecords",
                column: "FileImportRecordId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileImports",
                table: "FileImports",
                column: "FileImportId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileImportRecords_FileImports_FileImportId",
                table: "FileImportRecords",
                column: "FileImportId",
                principalTable: "FileImports",
                principalColumn: "FileImportId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileImports_GoogleSheetImportTypes_FileImportTypeId",
                table: "FileImports",
                column: "FileImportTypeId",
                principalTable: "GoogleSheetImportTypes",
                principalColumn: "GoogleSheetImportTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileImportRecords_FileImports_FileImportId",
                table: "FileImportRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_FileImports_GoogleSheetImportTypes_FileImportTypeId",
                table: "FileImports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoogleSheetImportTypes",
                table: "GoogleSheetImportTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileImports",
                table: "FileImports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileImportRecords",
                table: "FileImportRecords");

            migrationBuilder.RenameTable(
                name: "GoogleSheetImportTypes",
                newName: "GoogleSheetImportType");

            migrationBuilder.RenameTable(
                name: "FileImports",
                newName: "FileImport");

            migrationBuilder.RenameTable(
                name: "FileImportRecords",
                newName: "FileImportRecord");

            migrationBuilder.RenameIndex(
                name: "IX_FileImports_FileImportTypeId",
                table: "FileImport",
                newName: "IX_FileImport_FileImportTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_FileImportRecords_FileImportId",
                table: "FileImportRecord",
                newName: "IX_FileImportRecord_FileImportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoogleSheetImportType",
                table: "GoogleSheetImportType",
                column: "GoogleSheetImportTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileImport",
                table: "FileImport",
                column: "FileImportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileImportRecord",
                table: "FileImportRecord",
                column: "FileImportRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileImport_GoogleSheetImportType_FileImportTypeId",
                table: "FileImport",
                column: "FileImportTypeId",
                principalTable: "GoogleSheetImportType",
                principalColumn: "GoogleSheetImportTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileImportRecord_FileImport_FileImportId",
                table: "FileImportRecord",
                column: "FileImportId",
                principalTable: "FileImport",
                principalColumn: "FileImportId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
