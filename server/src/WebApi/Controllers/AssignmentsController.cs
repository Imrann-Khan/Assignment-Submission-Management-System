using Application.Assignments.Create;
using Application.Assignments.Delete;
using Application.Assignments.GetById;
using Application.Assignments.List;
using Application.Assignments.SetStatus;
using Application.Assignments.Update;
using Application.Common.DTOs;
using Application.Common.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/assignments")]
[Authorize]
public class AssignmentsController : ApiControllerBase
{
    public AssignmentsController(ISender sender) : base(sender){}
    [HttpGet]
    public async Task<ActionResult<List<AssignmentDto>>> GetAll([FromQuery] GetAssignmentsQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAssignmentByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<AssignmentDto>> Create(CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<AssignmentDto>> Update(Guid id, UpdateAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteAssignmentCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> SetStatus(Guid id, SetAssignmentStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        await Sender.Send(command, cancellationToken);
        return NoContent();
    }
}
