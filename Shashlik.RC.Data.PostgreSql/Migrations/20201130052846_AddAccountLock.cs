using Microsoft.EntityFrameworkCore.Migrations;

namespace Shashlik.RC.Data.PostgreSql.Migrations
{
    public partial class AddAccountLock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountLocks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LockEnd = table.Column<long>(nullable: false),
                    LoginFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLocks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLocks");
        }
    }
}
