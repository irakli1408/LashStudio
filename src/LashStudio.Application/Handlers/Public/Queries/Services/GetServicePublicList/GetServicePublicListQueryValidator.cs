using FluentValidation;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList;

namespace LashStudio.Application.Handlers.Public.Queries.Services.GetServicePublicList
{
    public sealed class GetServicePublicListQueryValidator : AbstractValidator<GetServicePublicListQuery>
    {
        public GetServicePublicListQueryValidator()
        {
            // Если категория указана — проверяем, что это валидное значение enum.
            When(x => x.Category.HasValue, () =>
            {
                RuleFor(x => x.Category!.Value)
                    .IsInEnum()
                    .WithMessage("Unknown service category.");
            });
        }
    }
}
