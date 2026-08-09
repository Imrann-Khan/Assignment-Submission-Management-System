using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;

namespace Application.Submissions.List;

public record GetSubmissionsQuery(Guid? AssignmentId, Guid? StudentId, int? PageNumber, int? PageSize) : IRequest<PagedResult<SubmissionDto>>;
