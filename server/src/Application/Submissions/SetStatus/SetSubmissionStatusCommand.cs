using Application.Common.Messaging;
using Domain.Enums;

namespace Application.Submissions.SetStatus;

public record SetSubmissionStatusCommand(Guid Id, SubmissionStatus Status) : IRequest<Unit>;
