using LashStudio.Application.Contracts.Services;
using LashStudio.Domain.Services;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Create
{
    public record CreateServiceCommand(
     string Slug, ServiceCategory Category, LashExtensionVariant? Variant,
     decimal Price, int? DurationMinutes,
     List<ServiceLocaleDto> Locales) : IRequest<Guid>;
}
