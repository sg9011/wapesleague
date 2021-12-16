using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class UpdateServerButtonDefaultShowing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ButtonGroupType",
                table: "ServerButtonGroups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "ShowOneOutOfList",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "ShowAllAtTheSameTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ButtonGroupType",
                table: "ServerButtonGroups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "ShowAllAtTheSameTime",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "ShowOneOutOfList");
        }
    }
}
