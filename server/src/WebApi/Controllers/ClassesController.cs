using Application.Classes.Create;
using Application.Classes.Delete;
using Application.Classes.List;
using Application.Classes.Update;
using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/classes")]
[Authorize]
public class ClassesController : ApiControllerBase
{
    public ClassesController(ISender sender) : base(sender) {}
    [HttpGet]
    public async Task<ActionResult<PagedResult<ClassDto>>> GetAll([FromQuery] GetClassesQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ClassDto>> Create(CreateClassCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ClassDto>> Update(Guid id, UpdateClassCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteClassCommand(id), cancellationToken);
        return NoContent();
    }
}
