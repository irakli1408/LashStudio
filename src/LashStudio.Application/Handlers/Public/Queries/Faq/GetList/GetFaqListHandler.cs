using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Admin.Queries.Faq.Get;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Faq.GetList
{
    public sealed class GetFaqListHandler : IRequestHandler<GetFaqListQuery, List<FaqItemVm>>
    {
        private readonly IAppDbContext _db;
        public GetFaqListHandler(IAppDbContext db) => _db = db;

        public async Task<List<FaqItemVm>> Handle(GetFaqListQuery q, CancellationToken ct)
        {
            var culture = (q.Culture ?? "").Trim().ToLowerInvariant();

            var items = await _db.FaqItems
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
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
                .ToListAsync(ct);

            return items
                .Select(x =>
                {
                    var loc = x.Loc ?? x.Fallback;
                    return new FaqItemVm(
                        x.Id,
                        x.SortOrder,
                        loc?.Question ?? "",
                        loc?.Answer ?? ""
                    );
                })
                .Where(vm => !string.IsNullOrWhiteSpace(vm.Question))
                .ToList();
        }
    }
}
