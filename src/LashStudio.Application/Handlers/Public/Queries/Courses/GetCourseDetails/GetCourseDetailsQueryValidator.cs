using FluentValidation;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseDetails
{
    public sealed class GetCourseDetailsQueryValidator : AbstractValidator<GetCourseDetailsQuery>
    {
        public GetCourseDetailsQueryValidator()
        {
            RuleFor(x => x.Culture).OptionalCulture(); 
            RuleFor(x => x.Slug).RequiredSlug();
        }
    }
}
