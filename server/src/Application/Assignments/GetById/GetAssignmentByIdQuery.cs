using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Assignments.GetById;

public record GetAssignmentByIdQuery(Guid Id) : IRequest<AssignmentDto>;
