using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeSeoValueFromAboutEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeoDescription",
                table: "AboutPages");

            migrationBuilder.DropColumn(
                name: "SeoKeywordsCsv",
                table: "AboutPages");

            migrationBuilder.DropColumn(
                name: "SeoTitle",
                table: "AboutPages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeoDescription",
                table: "AboutPages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoKeywordsCsv",
                table: "AboutPages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoTitle",
                table: "AboutPages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
