using FluentValidation;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.AboutPerson
{
    public sealed class GetAboutPublicQueryValidator : AbstractValidator<GetAboutPublicQuery>
    {      
        public GetAboutPublicQueryValidator()
        {
            RuleFor(x => x.Culture).OptionalCulture(); 
        }
    }
}