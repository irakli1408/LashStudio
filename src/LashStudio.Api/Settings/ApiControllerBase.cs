using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{culture}/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly ISender Sender;
    protected ApiControllerBase(ISender sender) => Sender = sender;
}