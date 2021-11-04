using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class SessionDateCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "MixSessions",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "MixSessions");
        }
    }
}
