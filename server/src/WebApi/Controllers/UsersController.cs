using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Users.Create;
using Application.Users.List;
using Application.Users.SetActiveStatus;
using Application.Users.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ApiControllerBase
{
    public UsersController(ISender sender) : base(sender) {}
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> SetActiveStatus(Guid id, SetUserActiveStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        await Sender.Send(command, cancellationToken);
        return NoContent();
    }
}