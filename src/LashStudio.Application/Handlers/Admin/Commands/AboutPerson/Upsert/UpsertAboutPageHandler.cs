using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.AboutPerson;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert
{
        public sealed class UpsertAboutPageHandler : IRequestHandler<UpsertAboutPageCommand, long>
        {
            private readonly IAppDbContext _db;
            public UpsertAboutPageHandler(IAppDbContext db) => _db = db;

            public async Task<long> Handle(UpsertAboutPageCommand q, CancellationToken ct)
            {
                // одна запись AboutPage (single)
                var e = await _db.AboutPages
                    .Include(x => x.Locales)
                    .FirstOrDefaultAsync(ct);

                if (e is null)
                {
                    e = new AboutPage
                    {
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    _db.AboutPages.Add(e);
                }

                // основные поля
                e.IsActive = q.Model.IsActive;
                e.IsCover = q.Model.IsCover;
                e.SeoTitle = q.Model.SeoTitle;
                e.SeoDescription = q.Model.SeoDescription;
                e.SeoKeywordsCsv = q.Model.SeoKeywordsCsv;

                // --- СИНХРОНИЗАЦИЯ ЛОКАЛЕЙ ---
                var incoming = (q.Model.Locales ?? new()).ToDictionary(x => x.Culture);

                // 1) удалить локали, которых нет во входных данных
                var toRemove = e.Locales.Where(l => !incoming.ContainsKey(l.Culture)).ToList();
                if (toRemove.Count > 0)
                    _db.AboutPageLocales.RemoveRange(toRemove);

                // 2) добавить/обновить локали
                foreach (var dto in incoming.Values)
                {
                    var loc = e.Locales.FirstOrDefault(l => l.Culture == dto.Culture);
                    if (loc is null)
                    {
                        e.Locales.Add(new AboutPageLocale
                        {
                            Culture = dto.Culture,
                            Title = dto.Title,
                            SubTitle = dto.SubTitle,
                            BodyHtml = dto.BodyHtml
                        });
                    }
                    else
                    {
                        loc.Title = dto.Title;
                        loc.SubTitle = dto.SubTitle;
                        loc.BodyHtml = dto.BodyHtml;
                    }
                }
                // --- /СИНХРОНИЗАЦИЯ ЛОКАЛЕЙ ---

                await _db.SaveChangesAsync(ct);
                return e.Id;
            }
        }
    }
