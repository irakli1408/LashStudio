using Asp.Versioning;
using LashStudio.Application.Handlers.Auth.Command.MakeAdmin;
using LashStudio.Application.Handlers.Auth.Command.RemoveAdmin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/admin/users")]
    [Authorize(Policy = "RequireSuperAdmin")]

    public sealed class AdminUsersController : ControllerBase
    {
        private readonly IMediator _m;
        public AdminUsersController(IMediator m) => _m = m;

        [HttpPost("{id:long}/make-admin")]
        public async Task<IActionResult> MakeAdmin(long id, CancellationToken ct)
        { await _m.Send(new MakeAdminCommand(id), ct); return NoContent(); }

        [HttpDelete("{id:long}/make-admin")]
        public async Task<IActionResult> RemoveAdmin(long id, CancellationToken ct)
        { await _m.Send(new RemoveAdminCommand(id), ct); return NoContent(); }
    }
}
