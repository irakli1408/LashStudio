using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Settings.Upsert
{
    public record SettingValueInput(string? Culture, string Value);

    public record UpsertSiteSettingCommand(
        string Key,
        List<SettingValueInput> Values
    ) : IRequest;
}
