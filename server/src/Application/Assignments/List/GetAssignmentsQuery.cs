using Application.Common.DTOs;
using Application.Common.Messaging;
using Domain.Enums;

namespace Application.Assignments.List;

public record GetAssignmentsQuery(Guid? ClassId, Guid? SubjectId, AssignmentStatus? Status) : IRequest<List<AssignmentDto>>;
