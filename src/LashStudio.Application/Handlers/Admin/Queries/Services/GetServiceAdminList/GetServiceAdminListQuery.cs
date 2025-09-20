using LashStudio.Application.Contracts.Services;
using LashStudio.Domain.Services;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList
{
    public record GetServiceAdminListQuery(ServiceCategory? Category) : IRequest<List<ServiceListItemVm>>;
}
