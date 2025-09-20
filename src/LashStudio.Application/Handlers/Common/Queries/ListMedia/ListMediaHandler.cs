using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Queries.ListMedia
{
    public sealed class ListMediaHandler(IMediaAttachmentService svc) : IRequestHandler<ListMediaQuery, IReadOnlyList<MediaAttachment>>
    {
        public Task<IReadOnlyList<MediaAttachment>> Handle(ListMediaQuery r, CancellationToken ct)
            => svc.ListAsync(r.OwnerType, r.OwnerKey, ct);
    }
}
