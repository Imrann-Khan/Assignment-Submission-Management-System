using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;

namespace Application.TeacherAssignments.List;

public record GetTeacherSubjectAssignmentQuery(
    Guid? TeacherId,
    Guid? ClassId,
    int? PageNumber,
    int? PageSize
) : IRequest<PagedResult<TeacherSubjectAssignmentDto>>;
