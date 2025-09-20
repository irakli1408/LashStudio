using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.DetachMedia
{
    public sealed record DetachMediaCommand(MediaOwnerType OwnerType, string OwnerKey, long AssetId) : IRequest;
}
