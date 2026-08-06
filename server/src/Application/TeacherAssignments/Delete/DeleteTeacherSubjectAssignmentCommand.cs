using Application.Common.Messaging;

namespace Application.TeacherAssignments.Delete;

public record DeleteTeacherSubjectAssignmentCommand
(
    Guid Id
) : IRequest<Unit>;