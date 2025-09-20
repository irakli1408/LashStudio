using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Contacts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Create
{
    public sealed class CreateContactCtaHandler
    : IRequestHandler<CreateContactCtaCommand, long>
    {
        private readonly IAppDbContext _db;
        public CreateContactCtaHandler(IAppDbContext db) => _db = db;

        public async Task<long> Handle(CreateContactCtaCommand r, CancellationToken ct)
        {
            var profile = await _db.ContactProfiles.FirstOrDefaultAsync(ct)
                ?? throw new Exception("contact_profile_not_found");

            var e = new ContactCta
            {
                ContactProfileId = profile.Id,
                Kind = r.Dto.Kind,
                IsEnabled = r.Dto.IsEnabled,
                Order = r.Dto.Order,
                UrlOverride = string.IsNullOrWhiteSpace(r.Dto.UrlOverride) ? null : r.Dto.UrlOverride!.Trim(),
            };

            foreach (var l in r.Dto.Locales)
            {
                e.Locales.Add(new ContactCtaLocale { Culture = l.Culture.Trim(), Label = l.Label.Trim() });
            }

            _db.ContactCtas.Add(e);
            await _db.SaveChangesAsync(ct);
            return e.Id;
        }
    }
}
