using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Phoenix_The_Fall_Web_application.Migrations
{
    /// <inheritdoc />
    public partial class Granates1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GranateId",
                table: "ClassLoadouts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Granates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PrefabPath = table.Column<string>(type: "text", nullable: false),
                    ClassRestriction = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Granates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGranates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    WeaponId = table.Column<int>(type: "integer", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    TotalKills = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGranates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerGranates_Granates_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Granates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGranates_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLoadouts_GranateId",
                table: "ClassLoadouts",
                column: "GranateId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGranates_PlayerId",
                table: "PlayerGranates",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGranates_WeaponId",
                table: "PlayerGranates",
                column: "WeaponId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLoadouts_Granates_GranateId",
                table: "ClassLoadouts",
                column: "GranateId",
                principalTable: "Granates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLoadouts_Granates_GranateId",
                table: "ClassLoadouts");

            migrationBuilder.DropTable(
                name: "PlayerGranates");

            migrationBuilder.DropTable(
                name: "Granates");

            migrationBuilder.DropIndex(
                name: "IX_ClassLoadouts_GranateId",
                table: "ClassLoadouts");

            migrationBuilder.DropColumn(
                name: "GranateId",
                table: "ClassLoadouts");
        }
    }
}
