using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddGoogleSheetRecordType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartRow",
                table: "GoogleSheetImportType");

            migrationBuilder.AlterColumn<string>(
                name: "GoogleSheetId",
                table: "GoogleSheetImportType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasTitleRow",
                table: "GoogleSheetImportType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Range",
                table: "GoogleSheetImportType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecordType",
                table: "GoogleSheetImportType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecordType",
                table: "FileImport",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasTitleRow",
                table: "GoogleSheetImportType");

            migrationBuilder.DropColumn(
                name: "Range",
                table: "GoogleSheetImportType");

            migrationBuilder.DropColumn(
                name: "RecordType",
                table: "GoogleSheetImportType");

            migrationBuilder.DropColumn(
                name: "RecordType",
                table: "FileImport");

            migrationBuilder.AlterColumn<string>(
                name: "GoogleSheetId",
                table: "GoogleSheetImportType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "StartRow",
                table: "GoogleSheetImportType",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
