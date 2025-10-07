using LashStudio.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ResetPassword
{
    public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _um;
        public ResetPasswordHandler(UserManager<ApplicationUser> um) => _um = um;

        public async Task Handle(ResetPasswordCommand r, CancellationToken ct)
        {
            var user = await _um.FindByIdAsync(r.Dto.UserId)
                       ?? throw new Exception("invalid_token_or_user");

            var result = await _um.ResetPasswordAsync(user, r.Dto.Token, r.Dto.NewPassword);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
