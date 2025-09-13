using Asp.Versioning;
using LashStudio.Api.Contracts.Settings;
using LashStudio.Application.Handlers.Admin.Commands.Settings.Upsert;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/admin/settings")]
    public sealed class AdminSettingsController : ApiControllerBase
    {
        public AdminSettingsController(ISender sender) : base(sender) { }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] UpsertSiteSettingDto dto)
        {
            await Sender.Send(new UpsertSiteSettingCommand(
                dto.Key,
                dto.Values.Select(v => new SettingValueInput(v.Culture, v.Value)).ToList()
            ));
            return NoContent();
        }
    }
}
