﻿using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Contracts.Media;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Admin.Queries.Media.GetMediaLibrary
{
    public sealed class GetMediaLibraryHandler : IRequestHandler<GetMediaLibraryQuery, MediaLibraryVm>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;
        private readonly IMediaUrlBuilder _urlBuilder;

        public GetMediaLibraryHandler(
            IAppDbContext db,
            IOptions<MediaOptions> opt,
            IMediaUrlBuilder urlBuilder)
            => (_db, _opt, _urlBuilder) = (db, opt, urlBuilder);

        public async Task<MediaLibraryVm> Handle(GetMediaLibraryQuery q, CancellationToken ct)
        {
            // 1) Только то, что переводится в SQL — тянем enum из БД, НЕ строку
            var raw = await _db.MediaAssets
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.Id)
                .Select(m => new
                {
                    m.Id,
                    m.OriginalFileName,
                    m.Type,        
                    m.StoredPath
                })
                .ToListAsync(ct);

            // 2) Формируем VM уже в памяти (тут можно безопасно вызывать UrlBuilder)
            var items = raw.Select(r => new MediaItemVm(
                AssetId: r.Id,
                Name: r.OriginalFileName,
                MediaType: (int)r.Type,                 
                Url: $"{_opt.Value.RequestPath.TrimEnd('/')}/{r.StoredPath}"
                          .Replace("//", "/").Replace("\\", "/"))).ToList();

            var photos = items.Where(x => x.MediaType == (int)MediaType.Photo).ToList();
            var videos = items.Where(x => x.MediaType == (int)MediaType.Video).ToList();

            return new MediaLibraryVm(photos, videos);
        }
    }
}
