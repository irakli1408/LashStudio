using LashStudio.Application.Contracts.Contacts;
using LashStudio.Application.Handlers.Admin.Commands.Contacts.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Contacts.Upsert;
using LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Create;
using LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Delete;
using LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Reorder;
using LashStudio.Application.Handlers.Admin.Commands.ContactsCta.SetContactCta;
using LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Update;
using LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Upsert;
using LashStudio.Application.Handlers.Admin.Queries.Contacts.GetContactProfileAdmin;
using LashStudio.Application.Handlers.Admin.Queries.ContactsCta.GetContactCta;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Contacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/contacts")]
    public sealed class AdminContactsController : ControllerBase
    {
        private readonly IMediator _m;
        public AdminContactsController(IMediator m) => _m = m;

        // Профиль
        [HttpGet]
        public Task<ContactProfileAdminVm> Get(CancellationToken ct)
            => _m.Send(new GetContactProfileAdminQuery(), ct);

        [HttpPost]
        public Task Upsert([FromBody] ContactProfileUpsertDto dto, CancellationToken ct)
            => _m.Send(new UpsertContactProfileCommand(dto), ct);

        // Сообщения
        //[HttpGet("messages")]
        //public Task<PagedResult<ContactMessageVm>> Messages([FromQuery] ContactMessageStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        //    => _m.Send(new GetContactMessagesQuery(new ContactMessageFilter(status, page, pageSize)), ct);

        //[HttpPut("messages/{id:long}/status")]
        //public Task SetMessageStatus(long id, [FromBody] SetContactMessageStatusDto dto, CancellationToken ct)
        //    => _m.Send(new SetContactMessageStatusCommand(id, dto.Status), ct);

        [HttpDelete("messages/{id:long}")]
        public Task DeleteMessage(long id, CancellationToken ct)
            => _m.Send(new DeleteContactMessageCommand(id), ct);

        // CTA
        [HttpGet("ctas")]
        public Task<IReadOnlyList<ContactCtaAdminVm>> ListCtas(CancellationToken ct)
            => _m.Send(new GetContactCtasQuery(), ct);

        [HttpPost("ctas")]
        public Task<long> CreateCta([FromBody] ContactCtaCreateDto dto, CancellationToken ct)
            => _m.Send(new CreateContactCtaCommand(dto), ct);

        [HttpPut("ctas/{id:long}")]
        public Task UpdateCta(long id, [FromBody] ContactCtaUpdateDto dto, CancellationToken ct)
            => _m.Send(new UpdateContactCtaCommand(dto with { Id = id }), ct);

        [HttpPut("ctas/{id:long}/locales")]
        public Task UpsertCtaLocales(long id, [FromBody] IReadOnlyList<ContactCtaLocaleUpsertDto> locales, CancellationToken ct)
            => _m.Send(new UpsertContactCtaLocalesCommand(id, locales), ct);

        [HttpDelete("ctas/{id:long}")]
        public Task DeleteCta(long id, CancellationToken ct)
            => _m.Send(new DeleteContactCtaCommand(id), ct);

        [HttpPost("ctas/reorder")]
        public Task ReorderCtas([FromBody] ReorderContactCtasDto dto, CancellationToken ct)
            => _m.Send(new ReorderContactCtasCommand(dto.OrderedIds), ct);

        [HttpPut("ctas/{id:long}/enabled")]
        public Task SetCtaEnabled(long id, [FromBody] SetCtaEnabledDto dto, CancellationToken ct)
            => _m.Send(new SetContactCtaEnabledCommand(id, dto.IsEnabled), ct);
    }

}
