using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Common;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Post;
using LashStudio.Domain.Courses;
using LashStudio.Domain.Faq;
using LashStudio.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LashStudio.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{culture}/admin/publish")]
public sealed class AdminPublishController : ApiControllerBase
{
    public AdminPublishController(ISender sender) : base(sender) { }

    // единая точка, Swagger без конфликтов
    [HttpPut("{entity}/{id}")]
    public async Task<IActionResult> SetActive(string entity, string id, [FromQuery] bool active = true)
    {
        switch (entity.Trim().ToLowerInvariant())
        {
            case "service":
            case "services":
                if (!Guid.TryParse(id, out var serviceId))
                    return BadRequest(new { error = "invalid_guid" });
                await Sender.Send(new SetActiveCommand<Service, Guid>(serviceId, active));
                return NoContent();

            case "course":
            case "courses":
                if (!long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var courseId))
                    return BadRequest(new { error = "invalid_long" });
                await Sender.Send(new SetActiveCommand<Course, long>(courseId, active));
                return NoContent();

            case "faq":
                if (!long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var faqId))
                    return BadRequest(new { error = "invalid_long" });
                await Sender.Send(new SetActiveCommand<FaqItem, long>(faqId, active));
                return NoContent();

            case "post":
            case "posts":
                if (!int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var postId))
                    return BadRequest(new { error = "invalid_int" });
                await Sender.Send(new PublishPostCommand(postId, active));
                return NoContent();

            default:
                return NotFound(new { error = "unsupported_entity" });
        }
    }
}
