namespace LashStudio.Domain.Media
{
    public enum MediaOwnerType : short
    {
        About = 1,
        Course = 2,
        Post = 3,
        Service = 4,
        AboutPage = 5
    }

    public sealed class MediaAttachment
    {
        public long Id { get; set; }
        public MediaOwnerType OwnerType { get; set; }
        public string OwnerKey { get; set; } = null!;  

        public long MediaAssetId { get; set; }
        public int SortOrder { get; set; }
        public bool IsCover { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public MediaAsset MediaAsset { get; set; } = null!;

    }
}
