using FluentValidation;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs
{
    public sealed class GetBlogListQueryValidator : AbstractValidator<GetBlogListQuery>
    {
        public GetBlogListQueryValidator()
        {
            RuleFor(x => x.Culture).OptionalCulture(); 

            // Номер страницы: минимум 1
            //RuleFor(x => x.Page)
            //    .GreaterThanOrEqualTo(1)
            //    .WithMessage("Page must be >= 1.");

            //// Размер страницы: в разумных пределах (1..100)
            //RuleFor(x => x.PageSize)
            //    .InclusiveBetween(1, 100)
            //    .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}
