using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Queries.Settings.Read;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/settings")]
    public sealed class SettingsController : ApiControllerBase
    {
        public SettingsController(ISender sender) : base(sender) { }

        [HttpGet]
        public Task<List<SettingVm>> Get(string culture)
            => Sender.Send(new GetSiteSettingsQuery(culture));
    }
}
