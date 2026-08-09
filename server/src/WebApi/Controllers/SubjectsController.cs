using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;
using Application.Subjects.Create;
using Application.Subjects.Delete;
using Application.Subjects.List;
using Application.Subjects.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/subjects")]
[Authorize]
public class SubjectsController : ApiControllerBase
{
    public SubjectsController(ISender sender) : base(sender) {}
    [HttpGet]
    public async Task<ActionResult<PagedResult<SubjectDto>>> GetAll([FromQuery] GetSubjectsQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SubjectDto>> Create(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SubjectDto>> Update(Guid id, UpdateSubjectCommand command, CancellationToken cancellationToken)
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
        await Sender.Send(new DeleteSubjectCommand(id), cancellationToken);
        return NoContent();
    }
}
