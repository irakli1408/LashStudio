using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Update
{
    public sealed class UpdateContactCtaHandler
     : IRequestHandler<UpdateContactCtaCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public UpdateContactCtaHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(UpdateContactCtaCommand r, CancellationToken ct)
        {
            var e = await _db.ContactCtas.FirstOrDefaultAsync(x => x.Id == r.Dto.Id, ct)
                ?? throw new Exception("cta_not_found");

            e.Kind = r.Dto.Kind;
            e.IsEnabled = r.Dto.IsEnabled;
            e.UrlOverride = string.IsNullOrWhiteSpace(r.Dto.UrlOverride) ? null : r.Dto.UrlOverride!.Trim();

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
