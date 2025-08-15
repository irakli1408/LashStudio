using System.Globalization;
using LashStudio.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using AppLocOptions = LashStudio.Api.Config.LocalizationOptions;

namespace LashStudio.Api.Localization;

public sealed class DbStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IServiceProvider _sp;
    private readonly AppLocOptions _opt;

    public DbStringLocalizerFactory(IServiceProvider sp, IOptions<AppLocOptions> opt)
    {
        _sp = sp;
        _opt = opt.Value;
    }

    public IStringLocalizer Create(Type resourceSource) => Create(null, null);

    public IStringLocalizer Create(string? baseName, string? location)
    {
        var db = _sp.GetRequiredService<AppDbContext>();
        var cache = _sp.GetRequiredService<IMemoryCache>();
        var culture = CultureInfo.CurrentUICulture.Name;

        return new DbStringLocalizer(db, cache, culture, _opt.DefaultCulture);
    }
}
