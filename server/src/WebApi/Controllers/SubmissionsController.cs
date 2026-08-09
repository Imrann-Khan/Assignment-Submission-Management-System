using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;
using Application.Submissions.Grade;
using Application.Submissions.List;
using Application.Submissions.SetStatus;
using Application.Submissions.Submit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/submissions")]
[Authorize]
public class SubmissionsController : ApiControllerBase
{
    public SubmissionsController(ISender sender) : base(sender) {}
    [HttpGet]
    public async Task<ActionResult<PagedResult<SubmissionDto>>> GetAll([FromQuery] GetSubmissionsQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("submit")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<SubmissionDto>> Submit(SubmitAssignmentCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/grade")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<SubmissionDto>> Grade(Guid id, GradeSubmissionCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> SetStatus(Guid id, SetSubmissionStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match request body id.");
        }

        await Sender.Send(command, cancellationToken);
        return NoContent();
    }
}
