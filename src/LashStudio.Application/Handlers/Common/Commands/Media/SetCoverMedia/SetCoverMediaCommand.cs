using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.SetCoverMedia
{
    public sealed record SetCoverMediaCommand(MediaOwnerType OwnerType, string OwnerKey, long AssetId) : IRequest;
}
