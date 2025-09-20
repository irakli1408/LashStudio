using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.SetCoverMedia
{
    public sealed class SetCoverMediaHandler(IMediaAttachmentService svc) : IRequestHandler<SetCoverMediaCommand>
    {
        public Task Handle(SetCoverMediaCommand r, CancellationToken ct)
            => svc.SetCoverAsync(r.OwnerType, r.OwnerKey, r.AssetId, ct);
    }
}
