using Application.Common.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly ISender Sender;
    protected ApiControllerBase(ISender sender)
    {
        Sender = sender;
    }
}