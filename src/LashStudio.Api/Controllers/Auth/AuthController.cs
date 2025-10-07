using LashStudio.Application.Common.EmailSender.DTO;
using LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ForgotPassword;
using LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ResetPassword;
using LashStudio.Application.Handlers.Auth.Command;
using LashStudio.Application.Handlers.Auth.Command.Login;
using LashStudio.Application.Handlers.Auth.Command.Logout;
using LashStudio.Application.Handlers.Auth.Command.Refresh;
using LashStudio.Application.Handlers.Auth.Command.Register;
using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using LashStudio.Application.Handlers.Auth.Queries.Me;
using LashStudio.Application.Handlers.Auth.Queries.MeResponses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LashStudio.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IMediator _m;
        public AuthController(IMediator m) => _m = m;

        [HttpPost("register"), AllowAnonymous]
        public Task<AuthResponse> Register([FromBody] RegisterCommand cmd, CancellationToken ct) => _m.Send(cmd, ct);

        [HttpPost("login"), AllowAnonymous]
        public Task<AuthResponse> Login([FromBody] LoginCommand cmd, CancellationToken ct) => _m.Send(cmd, ct);

        [HttpGet("me"), Authorize]
        public Task<MeResponse> Me(CancellationToken ct)
        {
            var uid = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return _m.Send(new MeQuery(uid), ct);
        }

        [HttpPost("refresh"), AllowAnonymous]
        public Task<AuthResponse> Refresh([FromBody] RefreshCommand cmd, CancellationToken ct) => _m.Send(cmd, ct);

        [HttpPost("logout"), Authorize]
        public Task Logout([FromBody] LogoutCommand cmd, CancellationToken ct) => _m.Send(cmd, ct);

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> Forgot([FromBody] ForgotPasswordDto dto)
        {
            await _m.Send(new ForgotPasswordCommand(dto));
            return Ok(); // всегда 200
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordDto dto)
        {
            await _m.Send(new ResetPasswordCommand(dto));
            return NoContent();
        }
    }
}
