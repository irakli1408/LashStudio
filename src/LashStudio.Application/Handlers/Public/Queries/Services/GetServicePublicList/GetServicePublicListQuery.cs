using LashStudio.Application.Contracts.Services;
using LashStudio.Domain.Services;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList
{
    public record GetServicePublicListQuery(ServiceCategory? Category)
        : IRequest<List<ServiceListItemVm>>;
}
