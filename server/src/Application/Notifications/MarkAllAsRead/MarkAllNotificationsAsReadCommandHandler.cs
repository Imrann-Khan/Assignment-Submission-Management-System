using Application.Common.Interfaces;
using Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.MarkAllAsRead;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsAsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var unread = await _context.Notifications
                            .Where(n => n.UserId == userId && !n.IsRead)
                            .ToListAsync(cancellationToken);
        
        foreach(var notification in unread)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}