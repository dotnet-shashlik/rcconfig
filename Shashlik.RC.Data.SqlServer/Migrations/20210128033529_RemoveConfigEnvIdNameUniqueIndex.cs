using Microsoft.EntityFrameworkCore.Migrations;

namespace Shashlik.RC.Data.SqlServer.Migrations
{
    public partial class RemoveConfigEnvIdNameUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModifyRecords_Configs_ConfigId",
                table: "ModifyRecords");

            migrationBuilder.DropIndex(
                name: "IX_Configs_EnvId_Name",
                table: "Configs");

            migrationBuilder.CreateIndex(
                name: "IX_Configs_EnvId",
                table: "Configs",
                column: "EnvId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModifyRecords_Configs_ConfigId",
                table: "ModifyRecords",
                column: "ConfigId",
                principalTable: "Configs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModifyRecords_Configs_ConfigId",
                table: "ModifyRecords");

            migrationBuilder.DropIndex(
                name: "IX_Configs_EnvId",
                table: "Configs");

            migrationBuilder.CreateIndex(
                name: "IX_Configs_EnvId_Name",
                table: "Configs",
                columns: new[] { "EnvId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifyRecords_Configs_ConfigId",
                table: "ModifyRecords",
                column: "ConfigId",
                principalTable: "Configs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
