using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Admin.Queries.Faq.Get;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Faq.GetById
{
    public sealed class GetFaqByIdHandler : IRequestHandler<GetFaqByIdQuery, FaqItemVm>
    {
        private readonly IAppDbContext _db;
        public GetFaqByIdHandler(IAppDbContext db) => _db = db;

        public async Task<FaqItemVm> Handle(GetFaqByIdQuery q, CancellationToken ct)
        {
            var culture = (q.Culture ?? "").Trim().ToLowerInvariant();

            var item = await _db.FaqItems
                .AsNoTracking()
                .Where(x => x.IsActive && x.Id == q.Id)
                .Select(x => new
                {
                    x.Id,
                    x.SortOrder,
                    Loc = !string.IsNullOrEmpty(culture)
                        ? x.Locales
                            .Where(l => l.Culture == culture)
                            .Select(l => new { l.Question, l.Answer })
                            .FirstOrDefault()
                        : null,
                    Fallback = x.Locales
                        .OrderBy(l => l.Id)
                        .Select(l => new { l.Question, l.Answer })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);

            if (item is null)
                throw new KeyNotFoundException("faq_not_found");

            var loc = item.Loc ?? item.Fallback;

            var vm = new FaqItemVm(
                item.Id,
                item.SortOrder,
                loc?.Question ?? "",
                loc?.Answer ?? ""
            );

            if (string.IsNullOrWhiteSpace(vm.Question))
                throw new KeyNotFoundException("faq_not_found");

            return vm;
        }
    }
}
