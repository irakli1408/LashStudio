using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Commands.Media.ReorderMedia
{
    public sealed class ReorderMediaHandler(IMediaAttachmentService svc) : IRequestHandler<ReorderMediaCommand>
    {
        public Task Handle(ReorderMediaCommand r, CancellationToken ct)
            => svc.ReorderAsync(r.OwnerType, r.OwnerKey, r.AssetIdsInOrder, ct);
    }
}
