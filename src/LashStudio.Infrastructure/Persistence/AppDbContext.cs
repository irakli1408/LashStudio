using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Blog;
using LashStudio.Domain.Media;
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

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Post>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SlugDefault).HasMaxLength(256).IsRequired();
            e.HasMany(x => x.Locales).WithOne(x => x.Post).HasForeignKey(x => x.PostId);
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

        b.Entity<MediaAsset>(e =>
        {
            e.ToTable("MediaAssets");
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).IsRequired();
            e.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
            e.Property(x => x.StoredPath).HasMaxLength(300).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Type);
        });
    }
}
