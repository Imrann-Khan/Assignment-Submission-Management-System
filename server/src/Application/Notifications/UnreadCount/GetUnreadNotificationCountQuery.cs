using Application.Common.Messaging;

namespace Application.Notifications.UnreadCount;


public record GetUnreadNotificationCountQuery : IRequest<int>;