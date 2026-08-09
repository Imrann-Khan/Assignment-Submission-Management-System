using Application.Common.Messaging;

namespace Application.Notifications.MarkAsRead;

public record MarkNotificationAsReadCommand(Guid Id) : IRequest<Unit>;
