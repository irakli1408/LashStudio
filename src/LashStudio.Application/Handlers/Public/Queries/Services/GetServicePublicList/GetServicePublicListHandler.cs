using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetServicePublicListHandler
    : IRequestHandler<GetServicePublicListQuery, List<ServiceListItemVm>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentStateService _state;
    private readonly IMediaUrlBuilder _mediaUrls;

    public GetServicePublicListHandler(IAppDbContext db, ICurrentStateService state, IMediaUrlBuilder mediaUrls)
    {
        _db = db;
        _state = state;
        _mediaUrls = mediaUrls;
    }

    public async Task<List<ServiceListItemVm>> Handle(GetServicePublicListQuery q, CancellationToken ct)
    {
        var culture = _state.CurrentCulture;
        var neutral = !string.IsNullOrWhiteSpace(culture) && culture!.Length >= 2 ? culture[..2] : null;

        var rows = await _db.Services.AsNoTracking()
            .Where(s => s.IsActive && (!q.Category.HasValue || s.Category == q.Category))
            .Select(s => new
            {
                s.Id,
                s.Slug,
                s.Price,
                Title = s.Locales
                    .OrderBy(l => l.Culture == culture ? 0 : (neutral != null && l.Culture.StartsWith(neutral) ? 1 : 2))
                    .Select(l => l.Title)
                    .FirstOrDefault() ?? string.Empty,
                CoverAssetId = _db.MediaAttachments
                    .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == s.OwnerKey)
                    .OrderByDescending(a => a.IsCover)
                    .ThenBy(a => a.SortOrder)
                    .Select(a => (long?)a.MediaAssetId)   // <-- long?
                    .FirstOrDefault()
            })
            .OrderBy(x => x.Title)
            .ToListAsync(ct);

        return rows.Select(x =>
            new ServiceListItemVm(
                x.Id,
                x.Slug,
                x.Title,
                x.Price,
                x.CoverAssetId.HasValue ? _mediaUrls.Url(x.CoverAssetId.Value) : null
            )
        ).ToList();
    }
}
