using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Contacts;
using LashStudio.Domain.Contacts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Contacts.GetContactProfile
{
    public sealed class GetContactProfileHandler
    : IRequestHandler<GetContactProfileQuery, ContactProfileVm>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentStateService _state;

        public GetContactProfileHandler(IAppDbContext db, ICurrentStateService state)
        {
            _db = db;
            _state = state;
        }

        public async Task<ContactProfileVm> Handle(GetContactProfileQuery q, CancellationToken ct)
        {
            var culture = string.IsNullOrWhiteSpace(q.Culture) ? _state.CurrentCulture : q.Culture;

            var e = await _db.ContactProfiles
                .Include(x => x.Locales)
                .Include(x => x.Hours)
                .Include(x => x.Ctas).ThenInclude(c => c.Locales)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (e is null)
                return EmptyVm();

            // Локаль (с фоллбэком на нейтральную и первую попавшуюся)
            var l = PickLocale(e.Locales, culture);

            // Часы работы (опциональны)
            var hoursVm = (e.Hours ?? Enumerable.Empty<ContactBusinessHour>())
                .OrderBy(h => h.Day)
                .Select(h => new BusinessHourVm(
                    h.Day, h.IsClosed,
                    h.Open?.ToString("HH:mm"),
                    h.Close?.ToString("HH:mm")))
                .ToList();
            if (hoursVm.Count == 0) hoursVm = null;

            // CTA из БД (включённые, по порядку); Label — из локалей CTA, Url — UrlOverride или авто-сборка
            var ctasVm = BuildCtas(e, culture);

            return new ContactProfileVm(
                OrganizationName: l?.OrganizationName,
                Address: l?.AddressLine,
                HowToFindUs: l?.HowToFindUs,
                Phones: e.Phones ?? Array.Empty<string>(),
                EmailPrimary: e.EmailPrimary,
                EmailSales: e.EmailSales,
                Instagram: e.Instagram,
                Telegram: e.Telegram,
                WhatsApp: e.WhatsApp,
                MapLat: e.MapLat, MapLng: e.MapLng, MapZoom: e.MapZoom,
                Hours: hoursVm,
                Ctas: ctasVm,
                SeoTitle: e.SeoTitle, SeoDescription: e.SeoDescription
            );
        }

        private static ContactProfileLocale? PickLocale(IEnumerable<ContactProfileLocale> locales, string? culture)
        {
            if (locales is null) return null;
            if (string.IsNullOrWhiteSpace(culture)) return locales.FirstOrDefault();

            var exact = locales.FirstOrDefault(x => x.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));
            if (exact != null) return exact;

            var neutral = culture.Length >= 2 ? culture[..2] : culture;
            return locales.FirstOrDefault(x => x.Culture.StartsWith(neutral, StringComparison.OrdinalIgnoreCase))
                   ?? locales.FirstOrDefault();
        }

        private static string NormalizePhone(string raw)
            => new string((raw ?? string.Empty).Where(char.IsDigit).ToArray());

        private static string? AutoUrl(ContactProfile profile, CtaKind kind)
        {
            switch (kind)
            {
                case CtaKind.Instagram:
                    return string.IsNullOrWhiteSpace(profile.Instagram) ? null
                        : $"https://ig.me/m/{profile.Instagram.TrimStart('@')}";
                case CtaKind.Telegram:
                    return string.IsNullOrWhiteSpace(profile.Telegram) ? null
                        : $"https://t.me/{profile.Telegram.TrimStart('@')}";
                case CtaKind.WhatsApp:
                    return string.IsNullOrWhiteSpace(profile.WhatsApp) ? null
                        : $"https://wa.me/{NormalizePhone(profile.WhatsApp)}";
                case CtaKind.Phone:
                    var p = profile.Phones?.FirstOrDefault();
                    return string.IsNullOrWhiteSpace(p) ? null : $"tel:{p}";
                case CtaKind.Email:
                    return string.IsNullOrWhiteSpace(profile.EmailPrimary) ? null : $"mailto:{profile.EmailPrimary}";
                default:
                    return null;
            }
        }

        private static string? ResolveLabel(IEnumerable<ContactCtaLocale> locs, string? culture, CtaKind kind)
        {
            if (locs is null) return DefaultLabel(kind);
            if (string.IsNullOrWhiteSpace(culture))
                return locs.FirstOrDefault()?.Label ?? DefaultLabel(kind);

            var exact = locs.FirstOrDefault(x => x.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));
            if (exact != null) return exact.Label;

            var neutral = culture.Length >= 2 ? culture[..2] : culture;
            return locs.FirstOrDefault(x => x.Culture.StartsWith(neutral, StringComparison.OrdinalIgnoreCase))?.Label
                ?? locs.FirstOrDefault()?.Label
                ?? DefaultLabel(kind);
        }

        private static string DefaultLabel(CtaKind kind) => kind switch
        {
            CtaKind.Instagram => "Instagram",
            CtaKind.Telegram => "Telegram",
            CtaKind.WhatsApp => "WhatsApp",
            CtaKind.Phone => "Call",
            CtaKind.Email => "Email",
            _ => "Open"
        };

        private static IReadOnlyList<CtaVm> BuildCtas(ContactProfile e, string? culture)
        {
            var list = new List<CtaVm>();

            foreach (var cta in (e.Ctas ?? Enumerable.Empty<ContactCta>()).Where(c => c.IsEnabled).OrderBy(c => c.Order))
            {
                var label = ResolveLabel(cta.Locales, culture, cta.Kind);
                var url = !string.IsNullOrWhiteSpace(cta.UrlOverride) ? cta.UrlOverride!.Trim() : AutoUrl(e, cta.Kind);

                if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(url))
                {
                    list.Add(new CtaVm(
                        Kind: cta.Kind.ToString().ToLowerInvariant(),
                        Label: label!,
                        Url: url!));
                }
            }

            // Удалим возможные дубли по (Kind, Url)
            return list.GroupBy(x => (x.Kind, x.Url)).Select(g => g.First()).ToList();
        }

        private static ContactProfileVm EmptyVm() => new(
            OrganizationName: null, Address: null, HowToFindUs: null,
            Phones: Array.Empty<string>(),
            EmailPrimary: null, EmailSales: null,
            Instagram: null, Telegram: null, WhatsApp: null,
            MapLat: null, MapLng: null, MapZoom: 15,
            Hours: null,
            Ctas: Array.Empty<CtaVm>(),
            SeoTitle: null, SeoDescription: null);
    }
}
