using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
// ImageSharp
using Img = SixLabors.ImageSharp.Image;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Upload;

public sealed class UploadMediaHandler : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    private readonly IFileStorage _storage;
    private readonly IAppDbContext _db;
    private readonly MediaOptions _opt;

    public UploadMediaHandler(IFileStorage storage, IAppDbContext db, IOptions<MediaOptions> opt)
        => (_storage, _db, _opt) = (storage, db, opt.Value);

    public async Task<UploadMediaResult> Handle(UploadMediaCommand c, CancellationToken ct)
    {
        if (c.Length <= 0) throw new ArgumentException("empty_file");

        // Валидация расширений/размера
        var ext = Path.GetExtension(c.FileName).ToLowerInvariant();
        var allow = c.Type == MediaType.Photo ? _opt.AllowedImageExtensions : _opt.AllowedVideoExtensions;
        var maxMb = c.Type == MediaType.Photo ? _opt.MaxImageSizeMb : _opt.MaxVideoSizeMb;

        if (!allow.Contains(ext, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("unsupported_extension");
        if (c.Length > maxMb * 1024L * 1024L)
            throw new ArgumentException("file_too_large");

        // yyyy/MM
        var now = DateTime.UtcNow;
        var subfolder = $"{now:yyyy}/{now:MM}";
        var guid = $"{Guid.NewGuid():N}";

        // ----- ВИДЕО: сохраняем как есть -----
        if (c.Type == MediaType.Video)
        {
            var newName = $"{guid}{ext}";
            var relPath = await _storage.SaveAsync(c.File, subfolder, newName, ct);

            var asset = new MediaAsset
            {
                Type = c.Type,
                OriginalFileName = c.FileName,
                StoredPath = relPath,
                ContentType = c.ContentType,
                SizeBytes = c.Length,
                CreatedAtUtc = now,
                Extension = ext
            };

            _db.MediaAssets.Add(asset);
            await _db.SaveChangesAsync(ct);

            var url = BuildUrl(relPath);
            return new UploadMediaResult(asset.Id, asset.OriginalFileName, (int)asset.Type, url, null);
        }

        // ----- ФОТО: нормализуем в JPEG + создаём превью -----
        // читаем входное изображение
        await using var input = c.File.OpenReadStream();      
        using var img = await Img.LoadAsync(input, ct);
        var origW = img.Width;
        var origH = img.Height;

        // основной файл (JPEG, качество из опций)
        using var mainMs = new MemoryStream();
        var mainEncoder = new JpegEncoder { Quality = _opt.JpegQualityMain };
        await img.SaveAsJpegAsync(mainMs, mainEncoder, ct);
        mainMs.Position = 0;

        var finalExt = ".jpg";
        var mainName = $"{guid}{finalExt}";
        var mainRelPath = await _storage.SaveAsync(mainMs, subfolder, mainName, ct);

        // превью
        var maxW = _opt.ThumbMaxWidth;
        var scale = (double)maxW / origW;
        var thW = Math.Min(maxW, origW);
        var thH = (int)Math.Max(1, Math.Round(origH * scale));

        using var thumbImg = img.Clone(ctx =>
        {
            ctx.Resize(new ResizeOptions
            {
                Size = new Size(thW, thH),
                Mode = ResizeMode.Max,
                Sampler = KnownResamplers.Lanczos3
            });
        });

        using var thumbMs = new MemoryStream();
        var thumbEncoder = new JpegEncoder { Quality = _opt.JpegQualityThumb };
        await thumbImg.SaveAsJpegAsync(thumbMs, thumbEncoder, ct);
        thumbMs.Position = 0;

        var thumbName = $"000_{guid}_thumb{finalExt}"; // префикс, чтобы «шло первым» в папке
        var thumbRelPath = await _storage.SaveAsync(thumbMs, subfolder, thumbName, ct);

        // БД
        var asset2 = new MediaAsset
        {
            Type = MediaType.Photo,
            OriginalFileName = c.FileName,
            StoredPath = mainRelPath,
            ContentType = "image/jpeg",
            SizeBytes = mainMs.Length,        // размер уже нормализованного JPEG
            Width = origW,
            Height = origH,
            ThumbStoredPath = thumbRelPath,
            ThumbWidth = thW,
            ThumbHeight = thH,
            Extension = finalExt,
            CreatedAtUtc = now
        };

        _db.MediaAssets.Add(asset2);
        await _db.SaveChangesAsync(ct);

        var mainUrl = BuildUrl(asset2.StoredPath);
        var thumbUrl = BuildUrl(asset2.ThumbStoredPath!);

        return new UploadMediaResult(asset2.Id, asset2.OriginalFileName, (int)asset2.Type, mainUrl, thumbUrl);
    }

    private string BuildUrl(string relPath)
        => $"{_opt.RequestPath.TrimEnd('/')}/{relPath}".Replace("//", "/").Replace("\\", "/");
}
