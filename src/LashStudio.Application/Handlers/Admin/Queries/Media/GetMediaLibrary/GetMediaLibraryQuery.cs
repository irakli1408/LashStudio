using LashStudio.Application.Contracts.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Media.GetMediaLibrary
{
    public sealed record GetMediaLibraryQuery() : IRequest<MediaLibraryVm>;
}
