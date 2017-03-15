using Microsoft.EntityFrameworkCore.Migrations;

namespace WebController.Migrations
{
    public partial class testMigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogicalLink",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    InputId = table.Column<string>(nullable: true),
                    OutputId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogicalLink", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "LogicalNodeSerialized",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    JsonData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogicalNodeSerialized", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("LogicalLink");
            migrationBuilder.DropTable("LogicalNodeSerialized");
        }
    }
}
