using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Create;

public record CreateFaqItemCommand(
    bool IsActive,
    int SortOrder,
    List<FaqLocaleInput> Locales
) : IRequest<long>;


