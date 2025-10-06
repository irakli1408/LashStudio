using FluentValidation;
using LashStudio.Application.Validation;

namespace LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogBySlug
{
    public sealed class GetBlogBySlugQueryValidator : AbstractValidator<GetBlogBySlugQuery>
    {   
        public GetBlogBySlugQueryValidator()
        {
            RuleFor(x => x.Culture).RequiredCulture();
            RuleFor(x => x.Slug).RequiredSlug();
        }
    }
}
