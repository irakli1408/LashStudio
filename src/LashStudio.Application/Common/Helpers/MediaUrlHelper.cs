using LashStudio.Application.Common.Options;

namespace LashStudio.Application.Common.Helpers
{
    public static class MediaUrlHelper
    {
        public static string? ToUrl(MediaOptions options, string? rel)
        {
            if (string.IsNullOrWhiteSpace(rel))
                return null;

            return $"{options.RequestPath.TrimEnd('/')}/{rel}"
                .Replace("//", "/")
                .Replace("\\", "/");
        }
    }
}
