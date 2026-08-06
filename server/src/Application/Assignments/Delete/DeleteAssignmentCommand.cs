using Application.Common.Messaging;

namespace Application.Assignments.Delete;

public record DeleteAssignmentCommand(Guid Id) : IRequest<Unit>;
