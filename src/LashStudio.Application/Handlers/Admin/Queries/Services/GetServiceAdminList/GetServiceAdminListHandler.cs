using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList
{
    public sealed class GetServiceAdminListHandler
    : IRequestHandler<GetServiceAdminListQuery, List<ServiceListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentStateService _state;

        public GetServiceAdminListHandler(IAppDbContext db, ICurrentStateService state)
        {
            _db = db;
            _state = state;
        }

        public async Task<List<ServiceListItemVm>> Handle(GetServiceAdminListQuery q, CancellationToken ct)
        {
            var culture = _state.CurrentCulture;                 // напр. "ru-RU" или "ka"
            var neutral = !string.IsNullOrEmpty(culture) && culture.Length >= 2
                ? culture.Substring(0, 2)                        // "ru", "ka", "en" и т.п.
                : null;

            var raw = await _db.Services.AsNoTracking()
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
                .ToListAsync(ct);

            return raw
                .Select(x => new ServiceListItemVm(x.Id, x.Slug, x.Title ?? string.Empty, x.Price, CoverUrl: null))
                .OrderBy(x => x.Title)
                .ToList();
        }
    }
}
