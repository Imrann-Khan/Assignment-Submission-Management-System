using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;
using Domain.Enums;

namespace Application.Assignments.List;

public record GetAssignmentsQuery(Guid? ClassId, Guid? SubjectId, AssignmentStatus? Status, int? PageNumber, int? PageSize) : IRequest<PagedResult<AssignmentDto>>;
