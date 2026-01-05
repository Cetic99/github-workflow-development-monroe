using CashVault.WebAPI.FIlters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers;

[ApiController]
[ApiExceptionFilter]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
