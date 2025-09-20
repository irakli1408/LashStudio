using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.AttachMedia
{
    public sealed record AttachMediaCommand(MediaOwnerType OwnerType, string OwnerKey, long AssetId) : IRequest;
}
