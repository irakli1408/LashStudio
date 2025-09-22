using LashStudio.Application.Common.Media;
using LashStudio.Application.Contracts.Media;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Media.MediaAssets
{
    public sealed record GetMediaAssetsQuery(
     int Skip = 0,
     int Take = 20,
     string? Search = null,
     MediaType? Type = null,
     string? Extension = null,
     DateTime? FromUtc = null,
     DateTime? ToUtc = null,
     MediaAssetOrderBy OrderBy = MediaAssetOrderBy.CreatedDesc
 ) : IRequest<PagedResult<MediaAssetListItemVm>>;
}
