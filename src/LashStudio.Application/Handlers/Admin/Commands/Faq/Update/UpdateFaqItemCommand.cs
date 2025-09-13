using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Update
{
    public record UpdateFaqItemCommand(
        long Id,
        bool? IsActive,
        int? SortOrder,
        List<FaqLocaleInput>? Locales
    ) : IRequest;
}
