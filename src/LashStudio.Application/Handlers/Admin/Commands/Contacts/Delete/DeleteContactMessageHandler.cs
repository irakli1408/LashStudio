using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Contacts.Delete
{
    public sealed class DeleteContactMessageHandler
     : IRequestHandler<DeleteContactMessageCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public DeleteContactMessageHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(DeleteContactMessageCommand r, CancellationToken ct)
        {
            var e = await _db.ContactMessages.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
                ?? throw new Exception("contact_message_not_found");

            _db.ContactMessages.Remove(e);
            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
