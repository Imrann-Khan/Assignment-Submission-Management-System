using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.List;


public class GetMyNotificationQueryHandler : IRequestHandler<GetMyNotificationQuery, PagedResult<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public GetMyNotificationQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<NotificationDto>> Handle(GetMyNotificationQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        var query = _context.Notifications.Where(n => n.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.OrderByDescending(n => n.CreatedAt)
                                .Skip((pageNumber-1)*pageSize)
                                .Take(pageSize)
                                .Select(n => new NotificationDto(
                                    n.Id, 
                                    n.Type.ToString(), 
                                    n.Message, 
                                    n.IsRead, 
                                    n.CreatedAt, 
                                    n.RelatedAssignmentId
                                ))
                                .ToListAsync(cancellationToken);
        
        return new PagedResult<NotificationDto>(items, totalCount, pageNumber, pageSize);
    }
}