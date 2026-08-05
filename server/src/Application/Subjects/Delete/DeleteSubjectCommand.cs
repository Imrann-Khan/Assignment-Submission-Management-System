using Application.Common.Messaging;

namespace Application.Subjects.Delete;

public record DeleteSubjectCommand(Guid Id) : IRequest<Unit>;
