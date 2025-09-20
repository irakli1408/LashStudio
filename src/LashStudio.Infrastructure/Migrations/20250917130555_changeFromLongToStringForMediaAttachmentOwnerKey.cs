using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeFromLongToStringForMediaAttachmentOwnerKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "MediaAttachments");

            migrationBuilder.AddColumn<string>(
                name: "OwnerKey",
                table: "MediaAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerKey",
                table: "MediaAttachments");

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "MediaAttachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
