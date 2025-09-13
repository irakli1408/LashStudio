using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Settings.Read
{
    public sealed class GetSiteSettingsHandler : IRequestHandler<GetSiteSettingsQuery, List<SettingVm>>
    {
        private readonly IAppDbContext _db;
        public GetSiteSettingsHandler(IAppDbContext db) => _db = db;

        public async Task<List<SettingVm>> Handle(GetSiteSettingsQuery q, CancellationToken ct)
        {
            var culture = q.Culture.Trim().ToLowerInvariant();

            var all = await _db.SiteSettings
                .AsNoTracking()
                .Include(s => s.Values)
                .ToListAsync(ct);

            // правило: сначала значение на culture, затем invariant (null), затем любое имеющееся
            var result = all.Select(s =>
            {
                var v = s.Values.FirstOrDefault(x => x.Culture == culture)
                     ?? s.Values.FirstOrDefault(x => x.Culture == null)
                     ?? s.Values.FirstOrDefault();
                return new SettingVm(s.Key, v?.Value ?? "");
            })
            .Where(x => x.Value.Length > 0)
            .ToList();

            return result;
        }
    }
}
