using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Delete
{
    public sealed record DeleteMediaAssetCommand(long AssetId, bool Force = false) : IRequest; // удалить физически
}
