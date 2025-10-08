using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Media;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Media.GetMediaLibrary
{
    public sealed class GetMediaLibraryHandler : IRequestHandler<GetMediaLibraryQuery, MediaLibraryVm>
    {
        private readonly IAppDbContext _db;
        public GetMediaLibraryHandler(IAppDbContext db) => _db = db;

        public async Task<MediaLibraryVm> Handle(GetMediaLibraryQuery q, CancellationToken ct)
        {
          var items = await _db.MediaAssets
            .AsNoTracking()
            .Where(m => !m.IsDeleted)
            .OrderByDescending(m => m.Id) 
            .Select(m => new MediaItemVm(
                m.Id,
                m.OriginalFileName,
                m.Type == MediaType.Video ? "video"
              : m.Type == MediaType.Photo ? "photo"
              : "other"
            )).ToListAsync(ct);
                
            var photos = items.Where(x => x.MediaType == "photo").ToList();
            var videos = items.Where(x => x.MediaType == "video").ToList();

            return new MediaLibraryVm(photos, videos);
        }
    }
}

