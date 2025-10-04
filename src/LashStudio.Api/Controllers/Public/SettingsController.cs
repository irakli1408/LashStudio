using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Queries.Settings.Read;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LashStudio.Api.Controllers.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/settings")]
    public sealed class SettingsController : ApiControllerBase
    {
        public SettingsController(ISender sender) : base(sender) { }

        [HttpGet]
        [OutputCache(PolicyName = "public-10m-tagged")]
        public Task<List<SettingVm>> Get(string culture)
            => Sender.Send(new GetSiteSettingsQuery(culture));
    }
}
