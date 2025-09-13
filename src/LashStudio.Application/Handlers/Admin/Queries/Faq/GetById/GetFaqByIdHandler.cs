using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Faq.GetById
{
    public sealed class GetFaqByIdHandler : IRequestHandler<GetFaqByIdQuery, FaqItemAdminVm>
    {
        private readonly IAppDbContext _db;
        public GetFaqByIdHandler(IAppDbContext db) => _db = db;

        public async Task<FaqItemAdminVm> Handle(GetFaqByIdQuery q, CancellationToken ct)
        {
            var item = await _db.FaqItems
                 .AsNoTracking()
                 .Include(x => x.Locales)
                 .FirstOrDefaultAsync(x => x.Id == q.Id, ct)
                 ?? throw new KeyNotFoundException("faq_not_found");

            var locales = item.Locales.AsEnumerable();

            if (q.OnlyThisCulture && !string.IsNullOrWhiteSpace(q.Culture))
            {
                var culture = q.Culture.Trim().ToLowerInvariant();
                locales = locales.Where(l => l.Culture == culture);
            }

            return new FaqItemAdminVm(
                item.Id, item.IsActive, item.SortOrder,
                locales.OrderBy(l => l.Culture)
                       .Select(l => new FaqLocaleVm(l.Id, l.Culture, l.Question, l.Answer))
                       .ToList());
        }
    }
}
