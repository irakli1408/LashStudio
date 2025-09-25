using LashStudio.Domain.AboutPerson;
using LashStudio.Domain.Blog;
using LashStudio.Domain.Contacts;
using LashStudio.Domain.Courses;
using LashStudio.Domain.Faq;
using LashStudio.Domain.Media;
using LashStudio.Domain.Services;
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
        DbSet<MediaAttachment> MediaAttachments { get; }

        //Faq
        DbSet<FaqItem> FaqItems { get; }
        DbSet<FaqItemLocale> FaqItemLocales { get; }

        //SiteSetting
        DbSet<SiteSetting> SiteSettings { get; }
        DbSet<SiteSettingValue> SiteSettingValues { get; }

        //Course
        DbSet<Course> Courses { get; }
        DbSet<CourseLocale> CourseLocales { get; }

        //Services
        DbSet<Service> Services { get; }
        DbSet<ServiceLocale> ServiceLocales { get; }

        //About
        DbSet<AboutPage> AboutPages { get; }
        DbSet<AboutPageLocale> AboutPageLocales { get; }

        //Contacts
        DbSet<ContactProfile> ContactProfiles { get; }
        DbSet<ContactProfileLocale> ContactProfileLocales { get; }
        DbSet<ContactBusinessHour> ContactBusinessHours { get; }
        DbSet<ContactMessage> ContactMessages { get; }
        DbSet<ContactCta> ContactCtas { get; }
        DbSet<ContactCtaLocale> ContactCtaLocales { get; }

    // Сохранение изменений
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
