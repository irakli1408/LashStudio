using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;       
using Microsoft.Extensions.Options;

namespace LashStudio.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IOptions<MediaOptions> opt, IHostEnvironment env)
    {
        _root = Path.Combine(env.ContentRootPath, opt.Value.RootPath);
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream file, string subfolder, string fileName, CancellationToken ct = default)
    {
        var folder = Path.Combine(_root, subfolder);
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, fileName);
        if (File.Exists(path))
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            fileName = $"{name}_{Guid.NewGuid():N}{ext}";
            path = Path.Combine(folder, fileName);
        }

        using var outStream = File.Create(path);
        await file.CopyToAsync(outStream, ct);

        return Path.Combine(subfolder, fileName).Replace('\\', '/');
    }
    public async Task<string> SaveAsync(IFormFile file, string subfolder, string fileName, CancellationToken ct)
    {
        using var s = file.OpenReadStream();
        return await SaveAsync(s, subfolder, fileName, ct);
    }

    public Task DeleteAsync(string subpath, CancellationToken ct = default)
    {
        var full = Path.Combine(_root, subpath);
        if (File.Exists(full)) File.Delete(full);
        return Task.CompletedTask;
    }
}
