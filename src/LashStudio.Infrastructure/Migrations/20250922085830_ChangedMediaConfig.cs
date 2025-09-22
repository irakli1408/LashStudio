using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMediaConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets");

            migrationBuilder.AlterColumn<string>(
                name: "StoredPath",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "MediaAssets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashSha256",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MediaAssets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAttachments_MediaAssetId",
                table: "MediaAttachments",
                column: "MediaAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAttachments_MediaAssets_MediaAssetId",
                table: "MediaAttachments",
                column: "MediaAssetId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaAttachments_MediaAssets_MediaAssetId",
                table: "MediaAttachments");

            migrationBuilder.DropIndex(
                name: "IX_MediaAttachments_MediaAssetId",
                table: "MediaAttachments");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "HashSha256",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MediaAssets");

            migrationBuilder.AlterColumn<string>(
                name: "StoredPath",
                table: "MediaAssets",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "MediaAssets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MediaAssets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets",
                column: "Type");
        }
    }
}
