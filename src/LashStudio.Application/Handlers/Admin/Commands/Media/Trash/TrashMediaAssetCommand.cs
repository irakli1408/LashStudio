using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Trash
{
    public sealed record TrashMediaAssetCommand(long AssetId) : IRequest;                 // пометить как удалённый (soft)
}
