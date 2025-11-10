using LashStudio.Domain.Media;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Upload
{
    public record UploadMediaCommand(
     MediaType Type,
     string FileName,
     string ContentType,
     long Length,
     IFormFile File   
 ) : IRequest<UploadMediaResult>;

    public record UploadMediaResult(long AssetId, string? Name, int MediaType, string Url, string? ThumbUrl);
}
