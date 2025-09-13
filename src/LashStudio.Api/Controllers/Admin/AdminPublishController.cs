using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Common;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Post;
using LashStudio.Domain.Faq;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{culture}/admin/publish")]
public sealed class AdminPublishController : ApiControllerBase
{
    public AdminPublishController(ISender sender) : base(sender) { }

    // Единая точка: /admin/publish/{entity}/{id}?active=true|false
    [HttpPut("{entity}/{id:long}")]
    public async Task<IActionResult> SetActive(string entity, long id, [FromQuery] bool active = true)
    {
        switch (entity.Trim().ToLowerInvariant())
        {
            case "faq":
                await Sender.Send(new SetActiveCommand<FaqItem, long>(id, active));
                return NoContent();

            // Раскомментируешь, когда появятся соответствующие сущности
            // case "service":
            // case "services":
            //     await Sender.Send(new SetActiveCommand<Service, long>(id, active));
            //     return NoContent();

            // case "portfolio":
            //     await Sender.Send(new SetActiveCommand<PortfolioItem, long>(id, active));
            //     return NoContent();

            // Блог — отдельный флоу (Status/PublishedAt)
            case "post":
            case "posts":
                await Sender.Send(new PublishPostCommand((int)id, active));
                return NoContent();

            default:
                return NotFound(new { error = "unsupported_entity" });
        }
    }
}
