using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;

namespace Application.Notifications.List;


public record GetMyNotificationQuery(
    int? PageNumber, 
    int? PageSize
) : IRequest<PagedResult<NotificationDto>>;