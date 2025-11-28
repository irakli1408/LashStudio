using Asp.Versioning;
using LashStudio.Application.Contracts.AboutPerson;
using LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Delete;
using LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert;
using LashStudio.Application.Handlers.Admin.Queries.AboutPerson;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/admin/about")]
    public sealed class AdminAboutController : ControllerBase
    {
        private readonly IMediator _m;
        public AdminAboutController(IMediator m) => _m = m;

        // READ (admin)
        [HttpGet]
        public Task<AboutAdminDto> Get(CancellationToken ct)
            => _m.Send(new GetAboutAdminQuery(), ct);

        // CREATE/UPDATE (single page → Upsert)
        [HttpPut]
        public Task<long> Upsert([FromBody] AboutUpsertCommandDto model, CancellationToken ct)
            => _m.Send(new UpsertAboutPageCommand(model), ct);

        // DELETE (опционально; если хочешь уметь удалять запись)
        [HttpDelete]
        public Task Delete(CancellationToken ct)
            => _m.Send(new DeleteAboutPageCommand(), ct);
    }
}
