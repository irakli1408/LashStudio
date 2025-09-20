using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.DetachMedia
{
    public sealed class DetachMediaHandler(IMediaAttachmentService svc) : IRequestHandler<DetachMediaCommand>
    {
        public Task Handle(DetachMediaCommand r, CancellationToken ct)
            => svc.DetachAsync(r.OwnerType, r.OwnerKey, r.AssetId, ct);
    }
}
