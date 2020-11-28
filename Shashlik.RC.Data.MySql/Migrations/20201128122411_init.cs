using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shashlik.RC.Data.MySql.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    Desc = table.Column<string>(maxLength: 512, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Envs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppId = table.Column<string>(maxLength: 32, nullable: false),
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    Desc = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Envs_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EnvId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Type = table.Column<string>(maxLength: 32, nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    Desc = table.Column<string>(maxLength: 512, nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleteTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Configs_Envs_EnvId",
                        column: x => x.EnvId,
                        principalTable: "Envs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IpWhites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ip = table.Column<string>(maxLength: 32, nullable: false),
                    EnvId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpWhites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpWhites_Envs_EnvId",
                        column: x => x.EnvId,
                        principalTable: "Envs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModifyRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ConfigId = table.Column<int>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    BeforeContent = table.Column<string>(nullable: true),
                    AfterContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifyRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModifyRecords_Configs_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configs_EnvId_Name",
                table: "Configs",
                columns: new[] { "EnvId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Envs_AppId_Name",
                table: "Envs",
                columns: new[] { "AppId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IpWhites_EnvId",
                table: "IpWhites",
                column: "EnvId");

            migrationBuilder.CreateIndex(
                name: "IX_ModifyRecords_ConfigId",
                table: "ModifyRecords",
                column: "ConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IpWhites");

            migrationBuilder.DropTable(
                name: "ModifyRecords");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Envs");

            migrationBuilder.DropTable(
                name: "Apps");
        }
    }
}
