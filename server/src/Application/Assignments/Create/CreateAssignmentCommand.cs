using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Assignments.Create;

public record CreateAssignmentCommand(
    string Title,
    string Description,
    Guid ClassId,
    Guid SubjectId,
    DateTime Deadline,
    int MaxMarks) : IRequest<AssignmentDto>;
