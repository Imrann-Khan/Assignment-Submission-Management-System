using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.TeacherAssignments.Create;
using Application.TeacherAssignments.Delete;
using Application.TeacherAssignments.List;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/teacher-assignments")]
[Authorize]
public class TeacherAssignmentsController : ApiControllerBase
{
    public TeacherAssignmentsController(ISender sender) : base(sender) {}
    [HttpGet]
    public async Task<ActionResult<List<TeacherSubjectAssignmentDto>>> GetAll([FromQuery] GetTeacherSubjectAssignmentQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TeacherSubjectAssignmentDto>> Create(CreateTeacherSubjectAssignmentCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteTeacherSubjectAssignmentCommand(id), cancellationToken);
        return NoContent();
    }
}
