using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeIsActiveValue_aboutPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCover",
                table: "AboutPages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCover",
                table: "AboutPages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
