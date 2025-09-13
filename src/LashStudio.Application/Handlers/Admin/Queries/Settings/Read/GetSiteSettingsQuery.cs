using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Settings.Read
{
    public record SettingVm(string Key, string Value);
    public record GetSiteSettingsQuery(string Culture) : IRequest<List<SettingVm>>;
}
