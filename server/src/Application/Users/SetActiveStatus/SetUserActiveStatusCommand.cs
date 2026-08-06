using Application.Common.Messaging;

namespace Application.Users.SetActiveStatus;

public record SetUserActiveStatusCommand(Guid Id, bool IsActive) : IRequest<Unit>;
