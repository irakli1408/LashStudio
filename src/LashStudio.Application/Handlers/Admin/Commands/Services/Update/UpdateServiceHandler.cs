using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Update
{
    public sealed class UpdateServiceHandler(IAppDbContext db) : IRequestHandler<UpdateServiceCommand>
    {
        public async Task Handle(UpdateServiceCommand m, CancellationToken ct)
        {
            var e = await db.Services
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(x => x.Id == m.Id, ct)
                ?? throw new NotFoundException("service_not_found");

            if (await db.Services.AnyAsync(x => x.Slug == m.Slug && x.Id != m.Id, ct))
                throw new ValidationException("slug_taken");

            if (m.Category != ServiceCategory.LashExtension && m.Variant != null)
                throw new ValidationException("variant_not_allowed_for_category");

            e.Slug = m.Slug;
            e.Category = m.Category;
            e.Variant = m.Variant;
            e.Price = m.Price;
            e.DurationMinutes = m.DurationMinutes;

            // локали: upsert по Culture
            var existingByCulture = e.Locales.ToDictionary(x => x.Culture);
            foreach (var l in m.Locales)
            {
                if (existingByCulture.TryGetValue(l.Culture, out var ex))
                {
                    ex.Title = l.Title;
                    ex.ShortDescription = l.ShortDescription;
                    ex.FullDescription = l.FullDescription;
                }
                else
                {
                    e.Locales.Add(new ServiceLocale
                    {
                        Id = Guid.NewGuid(),
                        ServiceId = e.Id,
                        Culture = l.Culture,
                        Title = l.Title,
                        ShortDescription = l.ShortDescription,
                        FullDescription = l.FullDescription
                    });
                }
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
