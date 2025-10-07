using LashStudio.Application.Common.EmailSender.DTO;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest;
}
