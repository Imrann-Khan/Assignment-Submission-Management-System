using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Submissions.SetStatus;

public class SetSubmissionStatusCommandHandler : IRequestHandler<SetSubmissionStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SetSubmissionStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SetSubmissionStatusCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Submission), request.Id);

        if (submission.Assignment.TeacherId != _currentUser.UserId)
        {
            throw new ForbiddenAccessException("You can only change the status of submissions for assignments you created.");
        }

        submission.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
