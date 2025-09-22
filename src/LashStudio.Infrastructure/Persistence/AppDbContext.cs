using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.AboutPerson;
using LashStudio.Domain.Blog;
using LashStudio.Domain.Contacts;
using LashStudio.Domain.Courses;
using LashStudio.Domain.Faq;
using LashStudio.Domain.Media;
using LashStudio.Domain.Services;
using LashStudio.Domain.Settings;
using LashStudio.Infrastructure.Localization;
using LashStudio.Infrastructure.Logs;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLocale> PostLocales => Set<PostLocale>();
    public DbSet<LogEntry> Logs => Set<LogEntry>();
    public DbSet<LocalizationResource> LocalizationResources => Set<LocalizationResource>();
    public DbSet<LocalizationValue> LocalizationValues => Set<LocalizationValue>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<FaqItem> FaqItems => Set<FaqItem>();
    public DbSet<FaqItemLocale> FaqItemLocales => Set<FaqItemLocale>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<SiteSettingValue> SiteSettingValues => Set<SiteSettingValue>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseLocale> CourseLocales => Set<CourseLocale>();
    public DbSet<CourseMedia> CourseMedia => Set<CourseMedia>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceLocale> ServiceLocales => Set<ServiceLocale>();
    public DbSet<ServiceMedia> ServiceMedia => Set<ServiceMedia>();
    public DbSet<MediaAttachment> MediaAttachments => Set<MediaAttachment>();
    public DbSet<AboutPage> AboutPages => Set<AboutPage>();
    public DbSet<AboutPageLocale> AboutPageLocales => Set<AboutPageLocale>();
    public DbSet<ContactProfile> ContactProfiles => Set<ContactProfile>();
    public DbSet<ContactProfileLocale> ContactProfileLocales => Set<ContactProfileLocale>();
    public DbSet<ContactBusinessHour> ContactBusinessHours => Set<ContactBusinessHour>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<ContactCta> ContactCtas { get; set; } = default!;
    public DbSet<ContactCtaLocale> ContactCtaLocales { get; set; } = default!;


    public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Post>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SlugDefault).HasMaxLength(256).IsRequired();
            e.HasMany(x => x.Locales).WithOne(x => x.Post).HasForeignKey(x => x.PostId);

            e.HasOne(x => x.CoverMedia)
             .WithMany()
             .HasForeignKey(x => x.CoverMediaId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<PostLocale>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Culture).HasMaxLength(10).IsRequired();
            e.Property(x => x.Title).HasMaxLength(256).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(256).IsRequired();
            e.HasIndex(x => new { x.Culture, x.Slug }).IsUnique();
        });

        b.Entity<Logs.LogEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Level).HasMaxLength(16);
            e.Property(x => x.Path).HasMaxLength(512);
            e.Property(x => x.Method).HasMaxLength(16);
            e.Property(x => x.TraceId).HasMaxLength(64);
        });

        b.Entity<LocalizationResource>(e =>
        {
            e.ToTable("LocalizationResources");
            e.HasKey(x => x.Id);
            e.Property(x => x.Key).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Key).IsUnique();
            e.Property(x => x.Description).HasMaxLength(512);
            e.HasMany(x => x.Values).WithOne(v => v.Resource).HasForeignKey(v => v.ResourceId);
        });

        b.Entity<LocalizationValue>(e =>
        {
            e.ToTable("LocalizationValues");
            e.HasKey(x => x.Id);
            e.Property(x => x.Culture).HasMaxLength(10).IsRequired();
            e.Property(x => x.Value).IsRequired();
            e.HasIndex(x => new { x.ResourceId, x.Culture }).IsUnique();
        });

        //b.Entity<MediaAsset>(e =>
        //{
        //    e.ToTable("MediaAssets");
        //    e.HasKey(x => x.Id);
        //    e.Property(x => x.Type).IsRequired();
        //    e.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
        //    e.Property(x => x.StoredPath).HasMaxLength(300).IsRequired();
        //    e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        //    e.HasIndex(x => x.Type);
        //});

        b.Entity<FaqItem>(e =>
        {
            e.ToTable("FaqItems");
            e.HasKey(x => x.Id);
            e.Property(x => x.IsActive).IsRequired();
            e.Property(x => x.SortOrder).IsRequired();
            e.HasMany(x => x.Locales)
             .WithOne(l => l.FaqItem)
             .HasForeignKey(l => l.FaqItemId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<FaqItemLocale>(e =>
        {
            e.ToTable("FaqItemLocales");
            e.HasKey(x => x.Id);
            e.Property(x => x.Culture).HasMaxLength(10).IsRequired();
            e.Property(x => x.Question).HasMaxLength(256).IsRequired();
            e.Property(x => x.Answer).IsRequired();
            e.HasIndex(x => new { x.FaqItemId, x.Culture }).IsUnique();
        });

        b.Entity<SiteSetting>(e =>
        {
            e.ToTable("SiteSettings");
            e.HasKey(x => x.Id);
            e.Property(x => x.Key).HasMaxLength(128).IsRequired();
            e.HasIndex(x => x.Key).IsUnique();
            e.HasMany(x => x.Values)
             .WithOne(v => v.Setting)
             .HasForeignKey(v => v.SiteSettingId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<SiteSettingValue>(e =>
        {
            e.ToTable("SiteSettingValues");
            e.HasKey(x => x.Id);
            e.Property(x => x.Culture).HasMaxLength(10); // может быть null
            e.Property(x => x.Value).IsRequired();
            e.HasIndex(x => new { x.SiteSettingId, x.Culture }).IsUnique();
        });
    }
}
