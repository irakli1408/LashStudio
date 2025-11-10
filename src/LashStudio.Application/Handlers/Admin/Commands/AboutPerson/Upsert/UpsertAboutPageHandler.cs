using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.AboutPerson;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert
{
    public sealed class UpsertAboutPageCommandHandler
        : IRequestHandler<UpsertAboutPageCommand, long>
    {
        private readonly IAppDbContext _db;

        public UpsertAboutPageCommandHandler(IAppDbContext db) { _db = db; }

        public async Task<long> Handle(UpsertAboutPageCommand request, CancellationToken ct)
        {
            var model = request.Model ?? throw new ArgumentNullException(nameof(request.Model));
            var now = DateTime.UtcNow;

            // Пытаемся найти существующую единственную страницу
            var aboutPage = await _db.AboutPages
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(ct);

            // --- CREATE (если нет ни одной записи) ---
            if (aboutPage is null)
            {
                aboutPage = new AboutPage
                {
                    IsActive = model.IsActive,
                    CreatedAtUtc = now,
                    PublishedAtUtc = model.IsActive ? now : null
                };

                foreach (var locale in model.Locales)
                {
                    aboutPage.Locales.Add(new AboutPageLocale
                    {
                        Culture = locale.Culture,
                        Title = locale.Title,
                        SubTitle = locale.SubTitle,
                        BodyHtml = locale.BodyHtml
                    });
                }

                _db.AboutPages.Add(aboutPage);
                await _db.SaveChangesAsync(ct);
                return aboutPage.Id;
            }

            // --- UPDATE (если запись уже есть) ---
            var wasActive = aboutPage.IsActive;

            aboutPage.IsActive = model.IsActive;

            if (!wasActive && model.IsActive && aboutPage.PublishedAtUtc is null)
                aboutPage.PublishedAtUtc = now;

            // --- MERGE LOCALES ---
            foreach (var dto in model.Locales)
            {
                var loc = aboutPage.Locales.FirstOrDefault(x => x.Culture == dto.Culture);
                if (loc == null)
                {
                    aboutPage.Locales.Add(new AboutPageLocale
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

            await _db.SaveChangesAsync(ct);
            return aboutPage.Id;
        }
    }
}
