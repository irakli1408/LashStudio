using FluentValidation;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.Contacts.GetContactProfile
{
    public sealed class GetContactProfileQueryValidator : AbstractValidator<GetContactProfileQuery>
    {    
        public GetContactProfileQueryValidator()
        {
            RuleFor(x => x.Culture).OptionalCulture(); 
        }
    }
}
