using Asp.Versioning;
using LashStudio.Application.Common.Media;
using LashStudio.Application.Contracts.Media;
using LashStudio.Application.Handlers.Admin.Commands.Media.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Media.Restore;
using LashStudio.Application.Handlers.Admin.Commands.Media.Trash;
using LashStudio.Application.Handlers.Admin.Commands.Media.Upload;
using LashStudio.Application.Handlers.Admin.Queries.Media.GetMediaLibrary;
using LashStudio.Application.Handlers.Common.Commands.Media.AttachMedia;
using LashStudio.Application.Handlers.Common.Commands.Media.DetachMedia;
using LashStudio.Application.Handlers.Common.Commands.Media.ReorderMedia;
using LashStudio.Application.Handlers.Common.Commands.Media.SetCoverMedia;
using LashStudio.Application.Handlers.Common.Queries.ListMedia;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Application.Handlers.Public.Queries.Media.MediaAssets;
using LashStudio.Application.Handlers.Public.Queries.Media.TrashMediaAssets;
using LashStudio.Domain.Media;                                // MediaOwnerType, MediaType, MediaAttachment
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{culture}/admin/media")]
public sealed class AdminMediaController : ApiControllerBase
{
    public AdminMediaController(ISender sender) : base(sender) { }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromQuery] string type, IFormFile file, CancellationToken ct)
    {
        if (file is null) return BadRequest(new { error = "file_required" });
        var mediaType = type?.ToLowerInvariant() == "video" ? MediaType.Video : MediaType.Photo;

        await using var stream = file.OpenReadStream();
        var res = await Sender.Send(new UploadMediaCommand(
            mediaType, file.FileName, file.ContentType, file.Length, stream), ct);

        return Created(res.PublicUrl, res);
    }

    [HttpDelete("{assetId:long}/trash")]
    public Task Trash(long assetId, CancellationToken ct)
       => Sender.Send(new TrashMediaAssetCommand(assetId), ct);

    [HttpPost("{assetId:long}/restore")]
    public Task Restore(long assetId, CancellationToken ct)
        => Sender.Send(new RestoreMediaAssetCommand(assetId), ct);

    [HttpDelete("{assetId:long}")]
    public Task HardDelete(long assetId, [FromQuery] bool force, CancellationToken ct)
        => Sender.Send(new DeleteMediaAssetCommand(assetId, force), ct);

    [HttpGet]
    public Task<PagedResult<MediaAssetListItemVm>> List(
       [FromQuery] int skip = 0,
       [FromQuery] int take = 20,
       [FromQuery] string? search = null,
       [FromQuery] MediaType? type = null,
       [FromQuery] string? extension = null,
       [FromQuery] DateTime? fromUtc = null,
       [FromQuery] DateTime? toUtc = null,
       [FromQuery] MediaAssetOrderBy orderBy = MediaAssetOrderBy.CreatedDesc,
       CancellationToken ct = default)
       => Sender.Send(new GetMediaAssetsQuery(skip, take, search, type, extension, fromUtc, toUtc, orderBy), ct);

    // Корзина
    [HttpGet("trash")]
    public Task<PagedResult<MediaAssetListItemVm>> Trash(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] string? search = null,
        [FromQuery] MediaType? type = null,
        [FromQuery] string? extension = null,
        [FromQuery] DateTime? deletedFromUtc = null,
        [FromQuery] DateTime? deletedToUtc = null,
        [FromQuery] MediaAssetOrderBy orderBy = MediaAssetOrderBy.CreatedDesc,
        CancellationToken ct = default)
        => Sender.Send(new GetTrashMediaAssetsQuery(skip, take, search, type, extension, deletedFromUtc, deletedToUtc, orderBy), ct);



// ↓↓↓ ключевые изменения — ownerKey:string без :long

[HttpPost("{ownerType}/{ownerKey}/{assetId:long}/attach")]
    public async Task<IActionResult> Attach(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct)
    {
        await Sender.Send(new AttachMediaCommand(ownerType, ownerKey, assetId), ct);
        return NoContent();
    }

    [HttpDelete("{ownerType}/{ownerKey}/{assetId:long}/detach")]
    public async Task<IActionResult> Detach(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct)
    {
        await Sender.Send(new DetachMediaCommand(ownerType, ownerKey, assetId), ct);
        return NoContent();
    }

    // Body: [assetId,...]
    [HttpPost("{ownerType}/{ownerKey}/reorder")]
    public async Task<IActionResult> Reorder(MediaOwnerType ownerType, string ownerKey, [FromBody] IReadOnlyList<long> assetIdsInOrder, CancellationToken ct)
    {
        if (assetIdsInOrder is null || assetIdsInOrder.Count == 0)
            return BadRequest(new { error = "assetIdsInOrder_required" });

        await Sender.Send(new ReorderMediaCommand(ownerType, ownerKey, assetIdsInOrder), ct);
        return NoContent();
    }

    [HttpPost("{ownerType}/{ownerKey}/{assetId:long}/cover")]
    public async Task<IActionResult> SetCover(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct)
    {
        await Sender.Send(new SetCoverMediaCommand(ownerType, ownerKey, assetId), ct);
        return NoContent();
    }

    [HttpGet("{ownerType}/{ownerKey}")]
    public Task<IReadOnlyList<MediaAttachment>> List(MediaOwnerType ownerType, string ownerKey, CancellationToken ct)
        => Sender.Send(new ListMediaQuery(ownerType, ownerKey), ct);

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary(CancellationToken ct)
    {
        var vm = await Sender.Send(new GetMediaLibraryQuery(), ct);
        return Ok(vm);
    }
}