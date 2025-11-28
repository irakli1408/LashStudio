using Asp.Versioning;
using LashStudio.Application.Contracts.Contacts;
using LashStudio.Application.Handlers.Public.Queries.Contacts.GetContactProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LashStudio.Api.Controllers.Public
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/public/contacts")]

    public sealed class PublicContactsController : ControllerBase
    {
        private readonly IMediator _m;
        public PublicContactsController(IMediator m) => _m = m;

        [HttpGet]
        [OutputCache(PolicyName = "public-10m-tagged")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client, VaryByHeader = "Accept-Language")]
        public Task<ContactProfileVm> Get([FromQuery] string? culture, CancellationToken ct)
            => _m.Send(new GetContactProfileQuery(culture), ct);
    }
}
