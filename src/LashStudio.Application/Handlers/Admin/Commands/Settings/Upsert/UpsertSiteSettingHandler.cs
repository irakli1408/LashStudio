using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Settings.Upsert
{
    public sealed class UpsertSiteSettingHandler : IRequestHandler<UpsertSiteSettingCommand>
    {
        private readonly IAppDbContext _db;
        public UpsertSiteSettingHandler(IAppDbContext db) => _db = db;

        public async Task Handle(UpsertSiteSettingCommand c, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(c.Key))
                throw new ArgumentException("key_required");

            var key = c.Key.Trim();
            var setting = await _db.SiteSettings
                .Include(s => s.Values)
                .FirstOrDefaultAsync(s => s.Key == key, ct);

            if (setting is null)
            {
                setting = new SiteSetting { Key = key };
                _db.SiteSettings.Add(setting);
            }

            foreach (var v in c.Values ?? [])
            {
                var culture = string.IsNullOrWhiteSpace(v.Culture)
                    ? null
                    : v.Culture.Trim().ToLowerInvariant();

                var existing = setting.Values.FirstOrDefault(x => x.Culture == culture);
                if (existing is null)
                    setting.Values.Add(new SiteSettingValue { Culture = culture, Value = v.Value ?? "" });
                else
                    existing.Value = v.Value ?? "";
            }

            await _db.SaveChangesAsync(ct);
            // (кэш можно добавить позже; сейчас не обязателен)
        }
    }
}