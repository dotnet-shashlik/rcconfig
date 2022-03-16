using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shashlik.RC.Server.Data.Sqlite.Migrations
{
    public partial class rc_0003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Environments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Environments");
        }
    }
}
