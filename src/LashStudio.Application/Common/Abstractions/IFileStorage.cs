namespace LashStudio.Application.Common.Abstractions;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream file, string subfolder, string fileName, CancellationToken ct = default);
    Task DeleteAsync(string subpath, CancellationToken ct = default);
}
