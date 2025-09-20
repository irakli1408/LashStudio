using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Delete
{
    public sealed class DeleteContactCtaHandler
     : IRequestHandler<DeleteContactCtaCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public DeleteContactCtaHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(DeleteContactCtaCommand r, CancellationToken ct)
        {
            var e = await _db.ContactCtas.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
                ?? throw new Exception("cta_not_found");

            _db.ContactCtas.Remove(e);
            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
