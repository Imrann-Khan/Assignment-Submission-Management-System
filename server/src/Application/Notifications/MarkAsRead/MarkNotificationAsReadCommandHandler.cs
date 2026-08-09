using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.MarkAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationAsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Notification), request.Id);

        if (notification.UserId != _currentUser.UserId)
        {
            throw new ForbiddenAccessException("You can only mark your own notifications as read.");
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
