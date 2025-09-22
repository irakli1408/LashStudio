using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Restore
{
    public sealed record RestoreMediaAssetCommand(long AssetId) : IRequest;               // восстановить из корзины
}
