using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.AttachMedia
{
    public sealed class AttachMediaHandler(IMediaAttachmentService svc) : IRequestHandler<AttachMediaCommand>
    {
        public Task Handle(AttachMediaCommand r, CancellationToken ct)
            => svc.AttachAsync(r.OwnerType, r.OwnerKey, r.AssetId, ct);
    }
}
