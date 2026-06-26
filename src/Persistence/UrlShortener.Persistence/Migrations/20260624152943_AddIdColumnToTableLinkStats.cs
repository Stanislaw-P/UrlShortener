using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdColumnToTableLinkStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinksStats_ShortUrls_ShortCode",
                table: "LinksStats");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ShortUrls_ShortCode",
                table: "ShortUrls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinksStats",
                table: "LinksStats");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "ShortUrls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "LinksStats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinksStats",
                table: "LinksStats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinksStats_ShortUrls_Id",
                table: "LinksStats",
                column: "Id",
                principalTable: "ShortUrls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinksStats_ShortUrls_Id",
                table: "LinksStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinksStats",
                table: "LinksStats");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LinksStats");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "ShortUrls",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ShortUrls_ShortCode",
                table: "ShortUrls",
                column: "ShortCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinksStats",
                table: "LinksStats",
                column: "ShortCode");

            migrationBuilder.AddForeignKey(
                name: "FK_LinksStats_ShortUrls_ShortCode",
                table: "LinksStats",
                column: "ShortCode",
                principalTable: "ShortUrls",
                principalColumn: "ShortCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
