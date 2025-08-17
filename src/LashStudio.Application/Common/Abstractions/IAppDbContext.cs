using LashStudio.Domain.Blog;
using LashStudio.Domain.Media;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Common.Abstractions;

public interface IAppDbContext
{  
        // Блог
        DbSet<Post> Posts { get; }
        DbSet<PostLocale> PostLocales { get; }

        // Медиа (фото/видео)
        DbSet<MediaAsset> MediaAssets { get; }

        // Сохранение изменений
        Task<int> SaveChangesAsync(CancellationToken ct = default);
}
