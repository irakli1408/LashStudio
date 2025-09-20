using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Contacts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Contacts.Upsert
{
    public sealed class UpsertContactProfileHandler
    : IRequestHandler<UpsertContactProfileCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public UpsertContactProfileHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(UpsertContactProfileCommand r, CancellationToken ct)
        {
            var e = await _db.ContactProfiles
                .Include(x => x.Locales)
                .Include(x => x.Hours)
                .FirstOrDefaultAsync(ct);

            if (e == null)
            {
                e = new ContactProfile();
                _db.ContactProfiles.Add(e);
            }

            e.EmailPrimary = r.Dto.EmailPrimary?.Trim();
            e.EmailSales = r.Dto.EmailSales?.Trim();
            e.Phones = r.Dto.Phones ?? Array.Empty<string>();
            e.Instagram = r.Dto.Instagram?.Trim().TrimStart('@');
            e.Telegram = r.Dto.Telegram?.Trim().TrimStart('@');
            e.WhatsApp = r.Dto.WhatsApp?.Trim();
            e.MapLat = r.Dto.MapLat;
            e.MapLng = r.Dto.MapLng;
            e.MapZoom = r.Dto.MapZoom;
            e.SeoTitle = r.Dto.SeoTitle?.Trim();
            e.SeoDescription = r.Dto.SeoDescription?.Trim();

            e.Locales.Clear();
            foreach (var l in r.Dto.Locales)
            {
                e.Locales.Add(new ContactProfileLocale
                {
                    Culture = l.Culture.Trim(),
                    OrganizationName = l.OrganizationName?.Trim(),
                    AddressLine = l.AddressLine?.Trim(),
                    HowToFindUs = l.HowToFindUs?.Trim()
                });
            }

            if (r.Dto.Hours != null)
            {
                e.Hours.Clear();
                foreach (var h in r.Dto.Hours)
                {
                    e.Hours.Add(new ContactBusinessHour
                    {
                        Day = h.Day,
                        IsClosed = h.IsClosed,
                        Open = TimeOnly.TryParse(h.Open, out var o) ? o : null,
                        Close = TimeOnly.TryParse(h.Close, out var c) ? c : null
                    });
                }
            }

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
