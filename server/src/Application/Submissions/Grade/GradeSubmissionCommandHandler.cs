using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Submissions.Grade;

public class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, SubmissionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GradeSubmissionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubmissionDto> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Submission), request.Id);

        if (submission.Assignment.TeacherId != _currentUser.UserId)
        {
            throw new ForbiddenAccessException("You can only grade submissions for assignments you created.");
        }

        submission.Marks = request.Marks;
        submission.Feedback = request.Feedback;
        submission.Status = SubmissionStatus.Graded;
        submission.GradedAt = DateTime.UtcNow;
        submission.GradedById = _currentUser.UserId;

        _context.Notifications.Add(new Notification
        {
            UserId = submission.StudentId,
            Type = NotificationType.SubmissionGraded,
            Message = $"Your submission for \"{submission.Assignment.Title}\" was graded: {request.Marks}/{submission.Assignment.MaxMarks}",
            RelatedAssignmentId = submission.AssignmentId
        });

        await _context.SaveChangesAsync(cancellationToken);

        return await _context.Submissions
            .Where(s => s.Id == submission.Id)
            .Select(s => new SubmissionDto(
                s.Id, s.AssignmentId, s.Assignment.Title, s.Assignment.MaxMarks, s.Assignment.Deadline,
                s.StudentId, s.Student.FullName, s.AnswerText, s.SubmittedAt, s.Status.ToString(),
                s.Marks, s.Feedback, s.GradedAt, s.GradedBy != null ? s.GradedBy.FullName : null))
            .SingleAsync(cancellationToken);
    }
}
