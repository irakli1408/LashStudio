using LashStudio.Domain.Blog;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Common.Abstractions;

public interface IAppDbContext
{
    DbSet<Post> Posts { get; }
    DbSet<PostLocale> PostLocales { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}
