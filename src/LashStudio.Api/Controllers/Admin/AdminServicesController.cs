using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Handlers.Admin.Commands.Services.Create;
using LashStudio.Application.Handlers.Admin.Commands.Services.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Services.Update;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList;
using LashStudio.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/services")]
    public sealed class AdminServicesController : ControllerBase
    {
        private readonly IMediator _m;
        public AdminServicesController(IMediator m) => _m = m;

        // LIST (admin)
        [HttpGet]
        public Task<List<ServiceListItemVm>> List([FromQuery] ServiceCategory? category, CancellationToken ct)
            => _m.Send(new GetServiceAdminListQuery(category), ct);

        // DETAILS (admin)
        [HttpGet("{id:guid}")]
        public Task<ServiceAdminDto> Details([FromRoute] Guid id, CancellationToken ct)
            => _m.Send(new GetServiceAdminDetailsQuery(id), ct);

        // CREATE
        [HttpPost]
        public Task<Guid> Create([FromBody] CreateServiceCommand cmd, CancellationToken ct)
            => _m.Send(cmd, ct);

        // UPDATE
        [HttpPut("{id:guid}")]
        public Task Update([FromRoute] Guid id, [FromBody] UpdateServiceCommand cmd, CancellationToken ct)
            => _m.Send(cmd with { Id = id }, ct);

        // DELETE
        [HttpDelete("{id:guid}")]
        public Task Delete([FromRoute] Guid id, CancellationToken ct)
            => _m.Send(new DeleteServiceCommand(id), ct);
    }
}
