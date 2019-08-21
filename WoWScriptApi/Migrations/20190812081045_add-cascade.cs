using Microsoft.EntityFrameworkCore.Migrations;

namespace WoWScriptApi.Migrations
{
    public partial class addcascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_ScriptItems_ScriptItemId",
                table: "Tags");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_ScriptItems_ScriptItemId",
                table: "Tags",
                column: "ScriptItemId",
                principalTable: "ScriptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_ScriptItems_ScriptItemId",
                table: "Tags");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_ScriptItems_ScriptItemId",
                table: "Tags",
                column: "ScriptItemId",
                principalTable: "ScriptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
