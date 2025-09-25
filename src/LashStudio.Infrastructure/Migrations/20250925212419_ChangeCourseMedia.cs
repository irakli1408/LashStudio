using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCourseMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerKey",
                table: "Courses",
                type: "varchar(36)",
                nullable: false,
                computedColumnSql: "LOWER(CONVERT(varchar(36), [Id]))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_OwnerKey",
                table: "Courses",
                column: "OwnerKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_OwnerKey",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "OwnerKey",
                table: "Courses");
        }
    }
}
