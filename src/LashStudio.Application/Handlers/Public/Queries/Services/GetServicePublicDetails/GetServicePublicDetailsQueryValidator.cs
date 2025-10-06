using FluentValidation;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.Services.GetServicePublicDetails
{
    public sealed class GetServicePublicDetailsQueryValidator : AbstractValidator<GetServicePublicDetailsQuery>
    {
        public GetServicePublicDetailsQueryValidator()
        {
            RuleFor(x => x.Slug).RequiredSlug();
        }
    }
}
