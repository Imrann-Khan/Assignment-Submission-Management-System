using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;

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

        entity.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
