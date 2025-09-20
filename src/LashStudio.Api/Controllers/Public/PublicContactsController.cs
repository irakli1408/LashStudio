using LashStudio.Application.Contracts.Contacts;
using LashStudio.Application.Handlers.Public.Queries.Contacts.GetContactProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Public
{
    [ApiController]
    [Route("api/public/contacts")]
    public sealed class PublicContactsController : ControllerBase
    {
        private readonly IMediator _m;
        public PublicContactsController(IMediator m) => _m = m;

        [HttpGet]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client, VaryByHeader = "Accept-Language")]
        public Task<ContactProfileVm> Get([FromQuery] string? culture, CancellationToken ct)
            => _m.Send(new GetContactProfileQuery(culture), ct);
    }
}
