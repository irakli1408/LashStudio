using FluentValidation;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseList
{
    public sealed class GetCourseListQueryValidator : AbstractValidator<GetCourseListQuery>
    {      
        public GetCourseListQueryValidator()
        {
            RuleFor(x => x.Culture).OptionalCulture();

            // Номер страницы: >= 1
            //RuleFor(x => x.Page)
            //    .GreaterThanOrEqualTo(1)
            //    .WithMessage("Page must be >= 1.");

            //// Размер страницы: 1..100 (подправьте при необходимости)
            //RuleFor(x => x.PageSize)
            //    .InclusiveBetween(1, 100)
            //    .WithMessage("PageSize must be between 1 and 100.");

            // Уровень: если передан — валидный enum
            When(x => x.Level.HasValue, () =>
            {
                RuleFor(x => x.Level!.Value)
                    .IsInEnum()
                    .WithMessage("Unknown course level.");
            });
        }
    }
}
