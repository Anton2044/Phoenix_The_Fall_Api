using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phoenix_The_Fall_Web_application.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_Weapons_GadgetId",
                table: "ClassLoadouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_Weapons_MainWeaponId",
                table: "ClassLoadouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_Weapons_SecondaryWeaponId",
                table: "ClassLoadouts");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_PlayerWeapons_GadgetId",
                table: "ClassLoadouts",
                column: "GadgetId",
                principalTable: "PlayerWeapons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_PlayerWeapons_MainWeaponId",
                table: "ClassLoadouts",
                column: "MainWeaponId",
                principalTable: "PlayerWeapons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_PlayerWeapons_SecondaryWeaponId",
                table: "ClassLoadouts",
                column: "SecondaryWeaponId",
                principalTable: "PlayerWeapons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_PlayerWeapons_GadgetId",
                table: "ClassLoadouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_PlayerWeapons_MainWeaponId",
                table: "ClassLoadouts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_PlayerWeapons_SecondaryWeaponId",
                table: "ClassLoadouts");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_Weapons_GadgetId",
                table: "ClassLoadouts",
                column: "GadgetId",
                principalTable: "Weapons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_Weapons_MainWeaponId",
                table: "ClassLoadouts",
                column: "MainWeaponId",
                principalTable: "Weapons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_Weapons_SecondaryWeaponId",
                table: "ClassLoadouts",
                column: "SecondaryWeaponId",
                principalTable: "Weapons",
                principalColumn: "Id");
        }
    }
}
