using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Delete
{
    public sealed class DeleteFaqItemHandler : IRequestHandler<DeleteFaqItemCommand>
    {
        private readonly IAppDbContext _db;
        public DeleteFaqItemHandler(IAppDbContext db) => _db = db;

        public async Task Handle(DeleteFaqItemCommand c, CancellationToken ct)
        {
            var item = await _db.FaqItems.FirstOrDefaultAsync(x => x.Id == c.Id, ct)
                       ?? throw new KeyNotFoundException("faq_not_found");

            _db.FaqItems.Remove(item); // каскадом удалит локали
            await _db.SaveChangesAsync(ct);
        }
    }
}