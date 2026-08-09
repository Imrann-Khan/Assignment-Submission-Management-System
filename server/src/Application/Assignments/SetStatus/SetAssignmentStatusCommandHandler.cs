using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Assignments.SetStatus;

public class SetAssignmentStatusCommandHandler : IRequestHandler<SetAssignmentStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SetAssignmentStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SetAssignmentStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Assignments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Assignment), request.Id);

        if (entity.TeacherId != _currentUser.UserId)
        {
            throw new ForbiddenAccessException("You can only change the status of assignments you created.");
        }

        var isNewlyPublished = request.Status == AssignmentStatus.Published && entity.Status != AssignmentStatus.Published;

        entity.Status = request.Status;

        if (isNewlyPublished)
        {
            var studentIds = await _context.Users
                .Where(u => u.ClassId == entity.ClassId && u.Role == UserRole.Student && u.IsActive)
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            foreach (var studentId in studentIds)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = studentId,
                    Type = NotificationType.AssignmentPublished,
                    Message = $"New assignment published: {entity.Title}",
                    RelatedAssignmentId = entity.Id
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
