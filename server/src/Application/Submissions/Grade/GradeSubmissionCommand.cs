using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Submissions.Grade;

public record GradeSubmissionCommand(Guid Id, int Marks, string? Feedback) : IRequest<SubmissionDto>;
