using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Submissions.Submit;

public record SubmitAssignmentCommand(Guid AssignmentId, string AnswerText) : IRequest<SubmissionDto>;
