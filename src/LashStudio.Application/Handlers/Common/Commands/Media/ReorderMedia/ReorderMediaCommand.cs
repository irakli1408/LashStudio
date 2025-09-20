using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.ReorderMedia
{
    public sealed record ReorderMediaCommand(MediaOwnerType OwnerType, string OwnerKey, IReadOnlyList<long> AssetIdsInOrder) : IRequest;
}
