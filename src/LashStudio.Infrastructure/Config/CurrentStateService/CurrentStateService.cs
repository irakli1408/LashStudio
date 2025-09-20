using LashStudio.Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace LashStudio.Infrastructure.Config.CurrentStateService
{
    public sealed class CurrentStateService : ICurrentStateService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentStateService(IHttpContextAccessor http) => _http = http;

        public string? CurrentCulture =>
            _http.HttpContext?.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture?.Name
            ?? Thread.CurrentThread.CurrentUICulture?.Name
            ?? Thread.CurrentThread.CurrentCulture?.Name;
    }
}
