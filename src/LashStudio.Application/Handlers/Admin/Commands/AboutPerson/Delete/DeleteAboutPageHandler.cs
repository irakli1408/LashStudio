using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Delete
{
    public sealed class DeleteAboutPageHandler : IRequestHandler<DeleteAboutPageCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public DeleteAboutPageHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(DeleteAboutPageCommand request, CancellationToken ct)
        {
            var e = await _db.AboutPages.FirstOrDefaultAsync(ct)
                ?? throw new NotFoundException("about_not_found", "about_not_found");

            _db.AboutPages.Remove(e);
            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
