using System.Globalization;

namespace LashStudio.Application.Common.Helpers
{
    public static class OwnerKeyHelper
    {
        public static string From(Guid id) => id.ToString("D").ToLowerInvariant();
        public static string From(long id) => id.ToString(CultureInfo.InvariantCulture);
        public static string From(int id) => id.ToString(CultureInfo.InvariantCulture);
        public static string From(string id) => id?.Trim() ?? string.Empty; // на случай, если уже строка
    }

    // (опц.) удобные расширения:
    public static class OwnerKeyExtensions
    {
        public static string ToOwnerKey(this Guid id) => OwnerKeyHelper.From(id);
        public static string ToOwnerKey(this long id) => OwnerKeyHelper.From(id);
        public static string ToOwnerKey(this int id) => OwnerKeyHelper.From(id);
        public static string ToOwnerKey(this string id) => OwnerKeyHelper.From(id);
    }
}
