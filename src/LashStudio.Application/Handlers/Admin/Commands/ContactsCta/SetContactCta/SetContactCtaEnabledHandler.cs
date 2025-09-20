using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.SetContactCta
{
    public sealed class SetContactCtaEnabledHandler
     : IRequestHandler<SetContactCtaEnabledCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public SetContactCtaEnabledHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(SetContactCtaEnabledCommand r, CancellationToken ct)
        {
            var e = await _db.ContactCtas.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
                ?? throw new Exception("cta_not_found");

            e.IsEnabled = r.IsEnabled;
            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
