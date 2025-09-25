﻿using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList
{
    public sealed class GetServiceAdminListHandler
    : IRequestHandler<GetServiceAdminListQuery, List<ServiceListItemWithMediaVm>>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentStateService _state;

        public GetServiceAdminListHandler(IAppDbContext db, ICurrentStateService state)
        {
            _db = db;
            _state = state;
        }

        public async Task<List<ServiceListItemWithMediaVm>> Handle(GetServiceAdminListQuery q, CancellationToken ct)
        {
            var culture = _state.CurrentCulture;
            var neutral = !string.IsNullOrEmpty(culture) && culture.Length >= 2 ? culture[..2] : null;

            var baseQuery = _db.Services.AsNoTracking()
                .Where(s => !q.Category.HasValue || s.Category == q.Category)
                .Select(s => new
                {
                    s.Id,
                    s.Slug,
                    s.Price,
                    s.OwnerKey,
                    Title = s.Locales
                        .OrderBy(l =>
                            l.Culture == culture ? 0 :
                            (neutral != null && l.Culture.StartsWith(neutral)) ? 1 : 2)
                        .Select(l => l.Title)
                        .FirstOrDefault() ?? ""
                });

            var ordered = baseQuery.OrderBy(x => x.Title);

            return await ordered
                .Select(x => new ServiceListItemWithMediaVm(
                    x.Id,
                    x.Slug,
                    x.Title,
                    x.Price,
                    _db.MediaAttachments
                        .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == x.OwnerKey)
                        .OrderBy(a => a.SortOrder)
                        .Select(a => new ServiceMediaVm(a.MediaAssetId, a.SortOrder, a.IsCover, null))
                        .ToList()
                ))
                .ToListAsync(ct);
        }
    }
}