using Microsoft.EntityFrameworkCore.Migrations;

namespace Shashlik.RC.Data.Sqlite.Migrations
{
    public partial class AddAccountLock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //5.0才支持
            // migrationBuilder.DropColumn(
            //     name: "NotifyUrl",
            //     table: "Envs");

            migrationBuilder.CreateTable(
                name: "AccountLocks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LockEnd = table.Column<long>(nullable: false),
                    LoginFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_AccountLocks", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLocks");

            // migrationBuilder.AddColumn<string>(
            //     name: "NotifyUrl",
            //     table: "Envs",
            //     maxLength: 512,
            //     nullable: true);
        }
    }
}