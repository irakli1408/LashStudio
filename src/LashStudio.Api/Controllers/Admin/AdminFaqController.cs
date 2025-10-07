using Asp.Versioning;
using LashStudio.Api.Contracts;
using LashStudio.Application.Contracts.Faq;
using LashStudio.Application.Handlers.Admin.Commands.Faq;
using LashStudio.Application.Handlers.Admin.Commands.Faq.Create;
using LashStudio.Application.Handlers.Admin.Commands.Faq.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Faq.Reorder;
using LashStudio.Application.Handlers.Admin.Commands.Faq.Update;
using LashStudio.Application.Handlers.Admin.Queries.Faq.Get;
using LashStudio.Application.Handlers.Admin.Queries.Faq.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{culture}/admin/faq")]
public sealed class AdminFaqController : ApiControllerBase
{
    public AdminFaqController(ISender sender) : base(sender) { }

    [HttpGet("{id:long}")]
    public Task<FaqItemAdminVm> GetById(long id, string culture, [FromQuery] bool onlyCulture = false)
    => Sender.Send(new GetFaqByIdQuery(id, culture, onlyCulture));

    [HttpGet]
    public async Task<ActionResult<List<FaqItemVm>>> Get(string culture)
        => await Sender.Send(new GetFaqListQuery(culture));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFaqDto dto)
    {
        var id = await Sender.Send(new CreateFaqItemCommand(
            dto.IsActive, dto.SortOrder,
            dto.Locales.Select(l => new FaqLocaleInput(l.Culture, l.Question, l.Answer)).ToList()
        ));
        return Created($"/api/v1/{{culture}}/admin/faq/{id}", new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateFaqDto dto)
    {
        await Sender.Send(new UpdateFaqItemCommand(
            id,
            dto.IsActive,
            dto.SortOrder,
            dto.Locales?.Select(l => new FaqLocaleInput(l.Culture, l.Question, l.Answer)).ToList()
        ));
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Sender.Send(new DeleteFaqItemCommand(id));
        return NoContent();
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder([FromBody] IReadOnlyList<FaqSortPairDto> body, CancellationToken ct)
    {
        await Sender.Send(new ReorderFaqItemsCommand(body), ct);
        return NoContent();
    }
}
