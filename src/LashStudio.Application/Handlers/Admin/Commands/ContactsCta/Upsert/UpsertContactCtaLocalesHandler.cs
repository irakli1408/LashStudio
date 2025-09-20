using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Contacts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Upsert
{
    public sealed class UpsertContactCtaLocalesHandler
    : IRequestHandler<UpsertContactCtaLocalesCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public UpsertContactCtaLocalesHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(UpsertContactCtaLocalesCommand r, CancellationToken ct)
        {
            var e = await _db.ContactCtas.Include(x => x.Locales).FirstOrDefaultAsync(x => x.Id == r.CtaId, ct)
                ?? throw new Exception("cta_not_found");

            e.Locales.Clear();
            foreach (var l in r.Locales)
                e.Locales.Add(new ContactCtaLocale { Culture = l.Culture.Trim(), Label = l.Label.Trim() });

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
