using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Delete
{
    public sealed class DeleteServiceHandler(IAppDbContext db) : IRequestHandler<DeleteServiceCommand>
    {
        public async Task Handle(DeleteServiceCommand m, CancellationToken ct)
        {
            var e = await db.Services.FirstOrDefaultAsync(x => x.Id == m.Id, ct)
                ?? throw new NotFoundException("service_not_found");
            db.Services.Remove(e);
            await db.SaveChangesAsync(ct);
        }
    }
}
