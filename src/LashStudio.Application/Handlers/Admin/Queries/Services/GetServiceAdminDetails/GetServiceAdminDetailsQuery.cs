using LashStudio.Application.Contracts.Services;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails
{
    public record GetServiceAdminDetailsQuery(Guid Id) : IRequest<ServiceAdminDto>;
}
