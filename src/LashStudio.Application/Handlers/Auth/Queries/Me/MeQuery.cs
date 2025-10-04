using LashStudio.Application.Handlers.Auth.Queries.MeResponses;

namespace LashStudio.Application.Handlers.Auth.Queries.Me
{
    public sealed record MeQuery(long UserId) : MediatR.IRequest<MeResponse>;
}
