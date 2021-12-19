using Microsoft.EntityFrameworkCore.Migrations;

namespace WaPesLeague.Data.Migrations
{
    public partial class AddServerSnipingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerSnipings",
                columns: table => new
                {
                    ServerSnipingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    IntervalAfterRegistrationOpeningInMinutes = table.Column<int>(type: "int", nullable: false),
                    SignUpDelayInMinutes = table.Column<int>(type: "int", nullable: false),
                    SignUpDelayDurationInHours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSnipings", x => x.ServerSnipingId);
                    table.ForeignKey(
                        name: "FK_ServerSnipings_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerSnipings_ServerId",
                table: "ServerSnipings",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerSnipings");
        }
    }
}
