using LashStudio.Application.Contracts.Services;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails
{
    public record GetServicePublicDetailsQuery(string Slug): IRequest<ServiceDetailsVm>;
}
