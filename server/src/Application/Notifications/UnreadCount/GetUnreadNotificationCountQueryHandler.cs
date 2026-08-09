using Application.Common.Interfaces;
using Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Application.Notifications.UnreadCount;

public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadNotificationCountQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(GetUnreadNotificationCountQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }
}