using LashStudio.Domain.Media;

namespace LashStudio.Domain.Blog;

public enum PostStatus { Draft = 0, Published = 1 }

public class Post
{
    public int Id { get; set; }
    public string SlugDefault { get; set; } = string.Empty;
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }

    public long? CoverMediaId { get; set; }
    public MediaAsset? CoverMedia { get; set; }

    public ICollection<PostLocale> Locales { get; set; } = new List<PostLocale>();
}

public class PostLocale
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Culture { get; set; } = "en";
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public Post Post { get; set; } = null!;
}
