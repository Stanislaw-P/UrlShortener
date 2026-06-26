using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdColumnLinkStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinksStats_ShortUrls_Id",
                table: "LinksStats");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LinksStats",
                newName: "ShortUrlId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinksStats_ShortUrls_ShortUrlId",
                table: "LinksStats",
                column: "ShortUrlId",
                principalTable: "ShortUrls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinksStats_ShortUrls_ShortUrlId",
                table: "LinksStats");

            migrationBuilder.RenameColumn(
                name: "ShortUrlId",
                table: "LinksStats",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinksStats_ShortUrls_Id",
                table: "LinksStats",
                column: "Id",
                principalTable: "ShortUrls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
