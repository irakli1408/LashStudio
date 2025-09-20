using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Contacts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Contacts.GetContactProfileAdmin
{
    public sealed class GetContactProfileAdminHandler
    : IRequestHandler<GetContactProfileAdminQuery, ContactProfileAdminVm>
    {
        private readonly IAppDbContext _db;
        public GetContactProfileAdminHandler(IAppDbContext db) => _db = db;

        public async Task<ContactProfileAdminVm> Handle(GetContactProfileAdminQuery q, CancellationToken ct)
        {
            var e = await _db.ContactProfiles
                .Include(x => x.Locales)
                .Include(x => x.Hours)
                .Include(x => x.Ctas).ThenInclude(c => c.Locales)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (e is null)
                return new ContactProfileAdminVm(
                    0, null, null, Array.Empty<string>(), null, null, null,
                    null, null, 15, Array.Empty<ContactBusinessHourAdminVm>(),
                    Array.Empty<ContactProfileLocaleVm>(), Array.Empty<ContactCtaAdminVm>(),
                    null, null);

            var hours = (e.Hours ?? []).OrderBy(h => h.Day)
                .Select(h => new ContactBusinessHourAdminVm(
                    h.Id, h.Day, h.IsClosed,
                    h.Open?.ToString("HH:mm"), h.Close?.ToString("HH:mm"))).ToList();

            var locales = (e.Locales ?? []).OrderBy(l => l.Culture)
                .Select(l => new ContactProfileLocaleVm(
                    l.Id, l.Culture, l.OrganizationName, l.AddressLine, l.HowToFindUs)).ToList();

            var ctas = (e.Ctas ?? []).OrderBy(c => c.Order)
                .Select(c => new ContactCtaAdminVm(
                    c.Id, c.Kind, c.IsEnabled, c.Order, c.UrlOverride,
                    c.Locales.OrderBy(l => l.Culture)
                        .Select(l => new ContactCtaLocaleVm(l.Id, l.Culture, l.Label)).ToList()
                )).ToList();

            return new ContactProfileAdminVm(
                e.Id, e.EmailPrimary, e.EmailSales, e.Phones ?? Array.Empty<string>(),
                e.Instagram, e.Telegram, e.WhatsApp,
                e.MapLat, e.MapLng, e.MapZoom,
                hours, locales, ctas, e.SeoTitle, e.SeoDescription);
        }
    }
}
