using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LashStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class serviceMediaChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_MediaAssets_CoverMediaId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAttachments_MediaAssets_MediaAssetId",
                table: "MediaAttachments");

            migrationBuilder.DropIndex(
                name: "IX_ServiceLocales_ServiceId",
                table: "ServiceLocales");

            migrationBuilder.DropIndex(
                name: "IX_CourseLocales_CourseId",
                table: "CourseLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContactProfileLocales_ContactProfileId",
                table: "ContactProfileLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContactCtas_ContactProfileId",
                table: "ContactCtas");

            migrationBuilder.DropIndex(
                name: "IX_ContactCtaLocales_ContactCtaId",
                table: "ContactCtaLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContactBusinessHours_ContactProfileId",
                table: "ContactBusinessHours");

            migrationBuilder.DropIndex(
                name: "IX_AboutPageLocales_AboutPageId",
                table: "AboutPageLocales");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Services",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Services",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ServiceLocales",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "ServiceLocales",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "MediaAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerKey",
                table: "MediaAttachments",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCover",
                table: "MediaAttachments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "MediaAttachments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "StoredPath",
                table: "MediaAssets",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PosterPath",
                table: "MediaAssets",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "MediaAssets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MediaAssets",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "HashSha256",
                table: "MediaAssets",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "MediaAssets",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "MediaAssets",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MediaAssets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Courses",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CourseLocales",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "CourseLocales",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WhatsApp",
                table: "ContactProfiles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telegram",
                table: "ContactProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instagram",
                table: "ContactProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailSales",
                table: "ContactProfiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailPrimary",
                table: "ContactProfiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "ContactProfileLocales",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HowToFindUs",
                table: "ContactProfileLocales",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "ContactProfileLocales",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine",
                table: "ContactProfileLocales",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "ContactMessages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ContactMessages",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ContactMessages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ContactMessages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientIp",
                table: "ContactMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UrlOverride",
                table: "ContactCtas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "ContactCtaLocales",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "ContactCtaLocales",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SeoTitle",
                table: "AboutPages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoKeywordsCsv",
                table: "AboutPages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoDescription",
                table: "AboutPages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCover",
                table: "AboutPages",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AboutPageLocales",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SubTitle",
                table: "AboutPageLocales",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "AboutPageLocales",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerKey",
                table: "Services",
                type: "varchar(36)",
                nullable: false,
                computedColumnSql: "LOWER(CONVERT(varchar(36), [Id]))",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Services_OwnerKey",
                table: "Services",
                column: "OwnerKey");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Slug",
                table: "Services",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocales_ServiceId_Culture",
                table: "ServiceLocales",
                columns: new[] { "ServiceId", "Culture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAttachments_OwnerType_OwnerKey",
                table: "MediaAttachments",
                columns: new[] { "OwnerType", "OwnerKey" },
                unique: true,
                filter: "[IsCover] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAttachments_OwnerType_OwnerKey_MediaAssetId",
                table: "MediaAttachments",
                columns: new[] { "OwnerType", "OwnerKey", "MediaAssetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_CreatedAtUtc",
                table: "MediaAssets",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Extension",
                table: "MediaAssets",
                column: "Extension");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_IsDeleted",
                table: "MediaAssets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Slug",
                table: "Courses",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseLocales_CourseId_Culture",
                table: "CourseLocales",
                columns: new[] { "CourseId", "Culture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactProfileLocales_ContactProfileId_Culture",
                table: "ContactProfileLocales",
                columns: new[] { "ContactProfileId", "Culture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Status_CreatedAtUtc",
                table: "ContactMessages",
                columns: new[] { "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactCtas_ContactProfileId_Order",
                table: "ContactCtas",
                columns: new[] { "ContactProfileId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactCtaLocales_ContactCtaId_Culture",
                table: "ContactCtaLocales",
                columns: new[] { "ContactCtaId", "Culture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactBusinessHours_ContactProfileId_Day",
                table: "ContactBusinessHours",
                columns: new[] { "ContactProfileId", "Day" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AboutPages_IsActive",
                table: "AboutPages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AboutPages_PublishedAtUtc",
                table: "AboutPages",
                column: "PublishedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AboutPageLocales_AboutPageId_Culture",
                table: "AboutPageLocales",
                columns: new[] { "AboutPageId", "Culture" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_MediaAssets_CoverMediaId",
                table: "Courses",
                column: "CoverMediaId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAttachments_MediaAssets_MediaAssetId",
                table: "MediaAttachments",
                column: "MediaAssetId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_MediaAssets_CoverMediaId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAttachments_MediaAssets_MediaAssetId",
                table: "MediaAttachments");

            migrationBuilder.DropIndex(
                name: "IX_Services_OwnerKey",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_Slug",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_ServiceLocales_ServiceId_Culture",
                table: "ServiceLocales");

            migrationBuilder.DropIndex(
                name: "IX_MediaAttachments_OwnerType_OwnerKey",
                table: "MediaAttachments");

            migrationBuilder.DropIndex(
                name: "IX_MediaAttachments_OwnerType_OwnerKey_MediaAssetId",
                table: "MediaAttachments");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_CreatedAtUtc",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_Extension",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_IsDeleted",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Slug",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_CourseLocales_CourseId_Culture",
                table: "CourseLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContactProfileLocales_ContactProfileId_Culture",
                table: "ContactProfileLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_Status_CreatedAtUtc",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactCtas_ContactProfileId_Order",
                table: "ContactCtas");

            migrationBuilder.DropIndex(
                name: "IX_ContactCtaLocales_ContactCtaId_Culture",
                table: "ContactCtaLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContactBusinessHours_ContactProfileId_Day",
                table: "ContactBusinessHours");

            migrationBuilder.DropIndex(
                name: "IX_AboutPages_IsActive",
                table: "AboutPages");

            migrationBuilder.DropIndex(
                name: "IX_AboutPages_PublishedAtUtc",
                table: "AboutPages");

            migrationBuilder.DropIndex(
                name: "IX_AboutPageLocales_AboutPageId_Culture",
                table: "AboutPageLocales");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerKey",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldComputedColumnSql: "LOWER(CONVERT(varchar(36), [Id]))");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ServiceLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "ServiceLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "MediaAttachments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerKey",
                table: "MediaAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCover",
                table: "MediaAttachments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "MediaAttachments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "StoredPath",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "PosterPath",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MediaAssets",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "HashSha256",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "MediaAssets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MediaAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "Courses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CourseLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "CourseLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "WhatsApp",
                table: "ContactProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telegram",
                table: "ContactProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instagram",
                table: "ContactProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailSales",
                table: "ContactProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailPrimary",
                table: "ContactProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "ContactProfileLocales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HowToFindUs",
                table: "ContactProfileLocales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "ContactProfileLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine",
                table: "ContactProfileLocales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientIp",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UrlOverride",
                table: "ContactCtas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "ContactCtaLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "ContactCtaLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "SeoTitle",
                table: "AboutPages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoKeywordsCsv",
                table: "AboutPages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoDescription",
                table: "AboutPages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCover",
                table: "AboutPages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AboutPageLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "SubTitle",
                table: "AboutPageLocales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Culture",
                table: "AboutPageLocales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocales_ServiceId",
                table: "ServiceLocales",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseLocales_CourseId",
                table: "CourseLocales",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactProfileLocales_ContactProfileId",
                table: "ContactProfileLocales",
                column: "ContactProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactCtas_ContactProfileId",
                table: "ContactCtas",
                column: "ContactProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactCtaLocales_ContactCtaId",
                table: "ContactCtaLocales",
                column: "ContactCtaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactBusinessHours_ContactProfileId",
                table: "ContactBusinessHours",
                column: "ContactProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AboutPageLocales_AboutPageId",
                table: "AboutPageLocales",
                column: "AboutPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_MediaAssets_CoverMediaId",
                table: "Courses",
                column: "CoverMediaId",
                principalTable: "MediaAssets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAttachments_MediaAssets_MediaAssetId",
                table: "MediaAttachments",
                column: "MediaAssetId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
