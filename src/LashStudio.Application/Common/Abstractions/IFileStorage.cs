using Microsoft.AspNetCore.Http;

namespace LashStudio.Application.Common.Abstractions;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream file, string subfolder, string fileName, CancellationToken ct = default); 
    Task<string> SaveAsync(IFormFile file, string subfolder, string fileName, CancellationToken ct);
    Task DeleteAsync(string subpath, CancellationToken ct = default);
}
