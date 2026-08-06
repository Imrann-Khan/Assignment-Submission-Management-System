using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Submissions.Submit;

public class SubmitAssignmentCommandHandler : IRequestHandler<SubmitAssignmentCommand, SubmissionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SubmitAssignmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubmissionDto> Handle(SubmitAssignmentCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId!.Value;

        var studentClassId = await _context.Users
            .Where(u => u.Id == studentId)
            .Select(u => u.ClassId)
            .SingleOrDefaultAsync(cancellationToken);

        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, cancellationToken);

        if (assignment is null || assignment.Status != AssignmentStatus.Published || assignment.ClassId != studentClassId)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        if (DateTime.UtcNow > assignment.Deadline)
        {
            throw new ForbiddenAccessException("The deadline for this assignment has passed.");
        }

        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.AssignmentId == request.AssignmentId && s.StudentId == studentId, cancellationToken);

        if (submission is null)
        {
            submission = new Submission
            {
                AssignmentId = request.AssignmentId,
                StudentId = studentId,
                AnswerText = request.AnswerText,
                SubmittedAt = DateTime.UtcNow,
                Status = SubmissionStatus.Submitted
            };
            _context.Submissions.Add(submission);
        }
        else
        {
            submission.AnswerText = request.AnswerText;
            submission.SubmittedAt = DateTime.UtcNow;
            submission.Status = SubmissionStatus.Submitted;
            submission.Marks = null;
            submission.Feedback = null;
            submission.GradedAt = null;
            submission.GradedById = null;
        }

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
