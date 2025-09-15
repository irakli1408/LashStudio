using LashStudio.Domain.Blog;
using LashStudio.Domain.Courses;
using LashStudio.Domain.Faq;
using LashStudio.Domain.Media;
using LashStudio.Domain.Settings;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Common.Abstractions;

public interface IAppDbContext
{
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        // Блог
        DbSet<Post> Posts { get; }
        DbSet<PostLocale> PostLocales { get; }

        // Медиа (фото/видео)
        DbSet<MediaAsset> MediaAssets { get; }

        //Faq
        DbSet<FaqItem> FaqItems { get; }
        DbSet<FaqItemLocale> FaqItemLocales { get; }

        //SiteSetting
        DbSet<SiteSetting> SiteSettings { get; }
        DbSet<SiteSettingValue> SiteSettingValues { get; }

        //Course
        DbSet<Course> Courses { get; }
        DbSet<CourseLocale> CourseLocales { get; }
        DbSet<CourseMedia> CourseMedia { get; }

    // Сохранение изменений
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
