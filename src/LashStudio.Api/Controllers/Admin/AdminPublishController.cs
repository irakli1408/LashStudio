using Asp.Versioning;
using LashStudio.Api.Helper.Enum;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Common;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Post;
using LashStudio.Domain.AboutPerson;
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
    [HttpPut("{entity:int}/{id}")]
    public async Task<IActionResult> SetActive(
     [FromRoute] int entity,
     [FromRoute] string id,
     [FromQuery] bool active = true)
    {
        if (!Enum.IsDefined(typeof(EntityKind), entity))
            return BadRequest(new { error = "unsupported_entity" });

        var kind = (EntityKind)entity;

        switch (kind)
        {
            case EntityKind.Service:
                if (!Guid.TryParse(id, out var serviceId))
                    return BadRequest(new { error = "invalid_guid" });
                await Sender.Send(new SetActiveCommand<Service, Guid>(serviceId, active));
                break;

            case EntityKind.Course:
                if (!long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var courseId))
                    return BadRequest(new { error = "invalid_long" });
                await Sender.Send(new SetActiveCommand<Course, long>(courseId, active));
                break;

            case EntityKind.Faq:
                if (!long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var faqId))
                    return BadRequest(new { error = "invalid_long" });
                await Sender.Send(new SetActiveCommand<FaqItem, long>(faqId, active));
                break;

            case EntityKind.Post:
                if (!int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var postId))
                    return BadRequest(new { error = "invalid_int" });
                await Sender.Send(new PublishPostCommand(postId, active));
                break;

            case EntityKind.AboutPage:
                if (!long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var aboutId))
                    return BadRequest(new { error = "invalid_long" });
                await Sender.Send(new SetActiveCommand<AboutPage, long>(aboutId, active));
                break;
        }

        return NoContent();
    }
}
