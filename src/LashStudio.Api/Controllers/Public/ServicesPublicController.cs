using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList;
using LashStudio.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Public
{
    [ApiController]
    [Route("api/services")]
    public sealed class ServicesPublicController : ControllerBase
    {
        private readonly IMediator _m;
        public ServicesPublicController(IMediator m) => _m = m;

        //Публичный список услуг (опционально по категории)
        [HttpGet]
        public Task<List<ServiceListItemVm>> List([FromQuery] ServiceCategory? category, CancellationToken ct)
            => _m.Send(new GetServicePublicListQuery(category), ct);

        //Публичные детали услуги по slug
        [HttpGet("{slug}")]
        public Task<ServiceDetailsVm> Details([FromRoute] string slug, CancellationToken ct)
            => _m.Send(new GetServicePublicDetailsQuery(slug), ct);
    }
}
