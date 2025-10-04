using LashStudio.Application.Contracts.AboutPerson;
using LashStudio.Application.Handlers.Public.Queries.AboutPerson;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LashStudio.Api.Controllers.Public
{
    [ApiController]
    [Route("api/public/about")]
    public sealed class PublicAboutController : ControllerBase
    {
        private readonly IMediator _m;
        public PublicAboutController(IMediator m) => _m = m;

        // /api/public/about?culture=ru-RU
        [HttpGet]
        [OutputCache(PolicyName = "public-10m-tagged")]
        [ProducesResponseType(typeof(AboutPublicVm), StatusCodes.Status200OK)]
        public Task<AboutPublicVm> Get([FromQuery] string? culture, CancellationToken ct)
            => _m.Send(new GetAboutPublicQuery(culture), ct);
    }
}
