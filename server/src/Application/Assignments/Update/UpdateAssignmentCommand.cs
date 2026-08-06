using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Assignments.Update;

public record UpdateAssignmentCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime Deadline,
    int MaxMarks) : IRequest<AssignmentDto>;
