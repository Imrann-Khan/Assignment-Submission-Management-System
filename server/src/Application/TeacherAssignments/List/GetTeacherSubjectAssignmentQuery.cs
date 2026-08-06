using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.TeacherAssignments.List;

public record GetTeacherSubjectAssignmentQuery(
    Guid? TeacherId,
    Guid? ClassId
) : IRequest<List<TeacherSubjectAssignmentDto>>;