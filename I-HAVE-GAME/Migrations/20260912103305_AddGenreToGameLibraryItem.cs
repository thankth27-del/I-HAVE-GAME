using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace I_HAVE_GAME.Migrations
{
    /// <inheritdoc />
    public partial class AddGenreToGameLibraryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "GameLibraryItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "GameLibraryItems");
        }
    }
}
