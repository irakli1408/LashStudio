using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Faq.Get
{
    public sealed class GetFaqListHandler : IRequestHandler<GetFaqListQuery, List<FaqItemVm>>
    {
        private readonly IAppDbContext _db;
        public GetFaqListHandler(IAppDbContext db) => _db = db;

        public async Task<List<FaqItemVm>> Handle(GetFaqListQuery q, CancellationToken ct)
        {
            var culture = q.Culture.Trim().ToLowerInvariant();

            var items = await _db.FaqItems
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.Locales)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
                .ToListAsync(ct);

            return items
                .Select(x =>
                {
                    var loc = x.Locales.FirstOrDefault(l => l.Culture == culture)
                              ?? x.Locales.FirstOrDefault(); 
                    return new FaqItemVm(
                        x.Id, x.SortOrder,
                        loc?.Question ?? "", loc?.Answer ?? "");
                })
                .Where(vm => !string.IsNullOrEmpty(vm.Question))
                .ToList();
        }
    }
}
