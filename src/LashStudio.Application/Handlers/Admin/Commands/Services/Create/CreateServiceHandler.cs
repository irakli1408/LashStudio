using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Create
{
    public sealed class CreateServiceHandler(IAppDbContext db) : IRequestHandler<CreateServiceCommand, Guid>
    {
        public async Task<Guid> Handle(CreateServiceCommand m, CancellationToken ct)
        {
            // Валидация: Slug уникален, Category/Variant согласованы
            if (await db.Services.AnyAsync(x => x.Slug == m.Slug, ct))
                throw new ValidationException("slug_taken");

            if (m.Category != ServiceCategory.LashExtension && m.Variant != null)
                throw new ValidationException("variant_not_allowed_for_category");

            var e = new Service
            {
                Id = Guid.NewGuid(),
                Slug = m.Slug,
                Category = m.Category,
                Variant = m.Variant,
                Price = m.Price,
                DurationMinutes = m.DurationMinutes,
                IsActive = false
            };
            e.Locales = m.Locales.Select(l => new ServiceLocale
            {
                Id = Guid.NewGuid(),
                Culture = l.Culture,
                Title = l.Title,
                ShortDescription = l.ShortDescription,
                FullDescription = l.FullDescription
            }).ToList();

            db.Services.Add(e);
            await db.SaveChangesAsync(ct);
            return e.Id;
        }
    }

}
