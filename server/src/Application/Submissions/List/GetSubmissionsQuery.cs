using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Submissions.List;

public record GetSubmissionsQuery(Guid? AssignmentId, Guid? StudentId) : IRequest<List<SubmissionDto>>;
