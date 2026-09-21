using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace I_HAVE_GAME.Migrations
{
    /// <inheritdoc />
    public partial class AddGameUpdatesAndDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Games was originally created outside EF migrations, so only add the new column.
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameUpdates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameUpdates_GameId_PublishedAt",
                table: "GameUpdates",
                columns: new[] { "GameId", "PublishedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameUpdates");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Games");
        }
    }
}
