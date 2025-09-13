using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CoverMediaId",
                table: "Posts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CoverMediaId",
                table: "Posts",
                column: "CoverMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_MediaAssets_CoverMediaId",
                table: "Posts",
                column: "CoverMediaId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_MediaAssets_CoverMediaId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CoverMediaId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CoverMediaId",
                table: "Posts");
        }
    }
}
