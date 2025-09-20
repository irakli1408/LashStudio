using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Common.Queries.ListMedia
{
    public sealed record ListMediaQuery(MediaOwnerType OwnerType, string OwnerKey) : IRequest<IReadOnlyList<MediaAttachment>>;
}
