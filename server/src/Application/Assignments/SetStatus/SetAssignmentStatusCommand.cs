using Application.Common.Messaging;
using Domain.Enums;

namespace Application.Assignments.SetStatus;

public record SetAssignmentStatusCommand(Guid Id, AssignmentStatus Status) : IRequest<Unit>;
