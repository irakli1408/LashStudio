using LashStudio.Domain.Media;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Upload
{
    public record UploadMediaCommand(
     MediaType Type,
     string FileName,
     string ContentType,
     long Length,
     Stream File
 ) : IRequest<UploadMediaResult>;

    public record UploadMediaResult(long Id, MediaType Type, string RelativePath, string PublicUrl);
}
