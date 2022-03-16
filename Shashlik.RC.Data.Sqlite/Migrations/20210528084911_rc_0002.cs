using Microsoft.EntityFrameworkCore.Migrations;

namespace Shashlik.RC.Data.Sqlite.Migrations
{
    public partial class rc_0002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ApplicationName",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "EnvironmentName",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Logs",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "LogUserName",
                table: "Logs",
                newName: "ResourceId");

            migrationBuilder.RenameColumn(
                name: "LogUserId",
                table: "Logs",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "Logs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogType",
                table: "Logs",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogType",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Logs",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "LogUserId");

            migrationBuilder.RenameColumn(
                name: "ResourceId",
                table: "Logs",
                newName: "LogUserName");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "Logs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Logs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationName",
                table: "Logs",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentId",
                table: "Logs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvironmentName",
                table: "Logs",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }
    }
}
