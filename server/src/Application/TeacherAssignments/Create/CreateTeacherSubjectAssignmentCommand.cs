using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.TeacherAssignments.Create;


public record CreateTeacherSubjectAssignmentCommand
(
    Guid TeacherId,
    Guid SubjectId
) : IRequest<TeacherSubjectAssignmentDto>;