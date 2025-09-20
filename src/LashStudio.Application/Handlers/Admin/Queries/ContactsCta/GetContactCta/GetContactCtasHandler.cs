using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Contacts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.ContactsCta.GetContactCta
{
    public sealed class GetContactCtasHandler
    : IRequestHandler<GetContactCtasQuery, IReadOnlyList<ContactCtaAdminVm>>
    {
        private readonly IAppDbContext _db;
        public GetContactCtasHandler(IAppDbContext db) => _db = db;

        public async Task<IReadOnlyList<ContactCtaAdminVm>> Handle(GetContactCtasQuery request, CancellationToken ct)
        {
            var ctas = await _db.ContactCtas
                .Include(x => x.Locales)
                .OrderBy(x => x.Order)
                .AsNoTracking()
                .ToListAsync(ct);

            return ctas.Select(x => new ContactCtaAdminVm(
                x.Id, x.Kind, x.IsEnabled, x.Order, x.UrlOverride,
                x.Locales.OrderBy(l => l.Culture).Select(l => new ContactCtaLocaleVm(l.Id, l.Culture, l.Label)).ToList()
            )).ToList();
        }
    }
}
