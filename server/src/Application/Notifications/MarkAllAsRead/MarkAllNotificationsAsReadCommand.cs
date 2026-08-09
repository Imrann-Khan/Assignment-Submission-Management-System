using Application.Common.Messaging;

namespace Application.Notifications.MarkAllAsRead;


public record MarkAllNotificationsAsReadCommand : IRequest<Unit>;
