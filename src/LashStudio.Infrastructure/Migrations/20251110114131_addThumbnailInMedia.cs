using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addThumbnailInMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThumbHeight",
                table: "MediaAssets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbStoredPath",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbWidth",
                table: "MediaAssets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbHeight",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "ThumbStoredPath",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "ThumbWidth",
                table: "MediaAssets");
        }
    }
}
