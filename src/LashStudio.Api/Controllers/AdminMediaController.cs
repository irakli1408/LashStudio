using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Commands.Media;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{culture}/admin/media")]
public sealed class AdminMediaController : ApiControllerBase
{
    public AdminMediaController(ISender sender) : base(sender) { }

    // POST /api/v1/{culture}/admin/media/upload?type=photo|video
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromQuery] string type, IFormFile file)
    {
        if (file is null) return BadRequest(new { error = "file_required" });

        var mediaType = type?.ToLowerInvariant() == "video" ? MediaType.Video : MediaType.Photo;

        await using var stream = file.OpenReadStream();
        var res = await Sender.Send(new UploadMediaCommand(
            mediaType, file.FileName, file.ContentType, file.Length, stream));

        return Created(res.PublicUrl, res);
    }
}
