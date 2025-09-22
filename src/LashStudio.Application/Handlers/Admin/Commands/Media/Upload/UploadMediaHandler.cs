using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Upload
{
    public sealed class UploadMediaHandler : IRequestHandler<UploadMediaCommand, UploadMediaResult>
    {
        private readonly IFileStorage _storage;
        private readonly IAppDbContext _db;
        private readonly MediaOptions _opt;

        public UploadMediaHandler(IFileStorage storage, IAppDbContext db, IOptions<MediaOptions> opt)
        { _storage = storage; _db = db; _opt = opt.Value; }

        public async Task<UploadMediaResult> Handle(UploadMediaCommand c, CancellationToken ct)
        {
            if (c.Length <= 0) throw new ArgumentException("empty_file");

            var ext = Path.GetExtension(c.FileName).ToLowerInvariant();
            var allow = c.Type == MediaType.Photo ? _opt.AllowedImageExtensions : _opt.AllowedVideoExtensions;
            var maxMb = c.Type == MediaType.Photo ? _opt.MaxImageSizeMb : _opt.MaxVideoSizeMb;

            if (!allow.Contains(ext, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("unsupported_extension");
            if (c.Length > maxMb * 1024L * 1024L)
                throw new ArgumentException("file_too_large");

            var subfolder = $"{DateTime.UtcNow:yyyy/MM}";
            var newName = $"{Guid.NewGuid():N}{ext}";
            var relPath = await _storage.SaveAsync(c.File, subfolder, newName, ct);

            var asset = new MediaAsset
            {
                Type = c.Type,
                OriginalFileName = c.FileName,
                StoredPath = relPath,
                ContentType = c.ContentType,
                SizeBytes = c.Length,
                CreatedAtUtc = DateTime.UtcNow
            };
            _db.MediaAssets.Add(asset);
            await _db.SaveChangesAsync(ct);

            var publicUrl = $"{_opt.RequestPath}/{relPath}".Replace("//", "/").Replace("\\", "/");
            return new UploadMediaResult(asset.Id, asset.Type, relPath, publicUrl);
        }
    }
}