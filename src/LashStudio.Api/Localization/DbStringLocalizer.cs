using LashStudio.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace LashStudio.Api.Localization;

public sealed class DbStringLocalizer : IStringLocalizer
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly string _culture;
    private readonly string _defaultCulture;

    public DbStringLocalizer(AppDbContext db, IMemoryCache cache, string culture, string defaultCulture)
    {
        _db = db;
        _cache = cache;
        _culture = culture.ToLowerInvariant();
        _defaultCulture = defaultCulture.ToLowerInvariant();
    }

    private string? GetCore(string key)
    {
        var cacheKey = $"localization:{_culture}:{key}";
        if (_cache.TryGetValue(cacheKey, out string? cached))
            return cached;

        var value = _db.LocalizationResources
            .Where(r => r.Key == key)
            .SelectMany(r => r.Values.Where(v => v.Culture == _culture))
            .Select(v => v.Value)
            .FirstOrDefault();


        if (value is null && _culture != _defaultCulture)
        {
            value = _db.LocalizationResources
                .Where(r => r.Key == key)
                .SelectMany(r => r.Values.Where(v => v.Culture == _defaultCulture))
                .Select(v => v.Value)
                .FirstOrDefault();
        }

        _cache.Set(cacheKey, value, TimeSpan.FromMinutes(10));
        return value;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var v = GetCore(name);
            return new LocalizedString(name, v ?? name, resourceNotFound: v is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.CurrentCulture, this[name].Value, arguments), resourceNotFound: false);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        Enumerable.Empty<LocalizedString>();

    public IStringLocalizer WithCulture(CultureInfo culture) =>
        new DbStringLocalizer(_db, _cache, culture.Name, _defaultCulture);
}
