using LashStudio.Application.Common.EmailSender.DTO;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ForgotPassword
{
    public sealed record ForgotPasswordCommand(ForgotPasswordDto Dto) : IRequest;
}
