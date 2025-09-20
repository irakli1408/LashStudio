using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Services.GetServicePublicList
{
    public sealed class GetServicePublicListHandler
    : IRequestHandler<GetServicePublicListQuery, List<ServiceListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentStateService _state;

        public GetServicePublicListHandler(IAppDbContext db, ICurrentStateService state)
        {
            _db = db;
            _state = state;
        }

        public async Task<List<ServiceListItemVm>> Handle(GetServicePublicListQuery q, CancellationToken ct)
        {
            var culture = _state.CurrentCulture;                           // напр. "ru-RU" или "ru"
            var neutral = !string.IsNullOrWhiteSpace(culture) && culture!.Length >= 2
                ? culture[..2]
                : null;

            var data = await _db.Services.AsNoTracking()
                .Where(s => s.IsActive)
                .Where(s => !q.Category.HasValue || s.Category == q.Category)
                .Select(s => new
                {
                    s.Id,
                    s.Slug,
                    s.Price,
                    s.CoverMediaId,
                    Title = s.Locales
                        .OrderBy(l =>
                            l.Culture == culture ? 0 :
                            (neutral != null && l.Culture.StartsWith(neutral)) ? 1 : 2)
                        .Select(l => l.Title)
                        .FirstOrDefault()
                })
                .OrderBy(x => x.Title)
                .ToListAsync(ct);

            return data.Select(x =>
                new ServiceListItemVm(x.Id, x.Slug, x.Title ?? string.Empty, x.Price, CoverUrl: null)
            ).ToList();
        }
    }
}
