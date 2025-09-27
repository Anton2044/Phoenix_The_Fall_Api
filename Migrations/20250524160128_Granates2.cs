using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phoenix_The_Fall_Web_application.Migrations
{
    /// <inheritdoc />
    public partial class Granates2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_Granates_GranateId",
                table: "ClassLoadouts");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_PlayerGranates_GranateId",
                table: "ClassLoadouts",
                column: "GranateId",
                principalTable: "PlayerGranates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_PlayerGranates_GranateId",
                table: "ClassLoadouts");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_Granates_GranateId",
                table: "ClassLoadouts",
                column: "GranateId",
                principalTable: "Granates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
