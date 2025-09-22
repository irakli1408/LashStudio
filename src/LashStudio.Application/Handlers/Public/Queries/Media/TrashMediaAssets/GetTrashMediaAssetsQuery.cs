using LashStudio.Application.Common.Media;
using LashStudio.Application.Contracts.Media;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Media.TrashMediaAssets
{
    public sealed record GetTrashMediaAssetsQuery(
     int Skip = 0,
     int Take = 20,
     string? Search = null,
     MediaType? Type = null,
     string? Extension = null,
     DateTime? DeletedFromUtc = null,
     DateTime? DeletedToUtc = null,
     MediaAssetOrderBy OrderBy = MediaAssetOrderBy.CreatedDesc
 ) : IRequest<PagedResult<MediaAssetListItemVm>>;
}
