using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Submissions.List;

public class GetSubmissionsQueryHandler : IRequestHandler<GetSubmissionsQuery, List<SubmissionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSubmissionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<SubmissionDto>> Handle(GetSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Submissions.AsQueryable();

        if (_currentUser.Role == UserRole.Student)
        {
            query = query.Where(s => s.StudentId == _currentUser.UserId);
        }
        else if (_currentUser.Role == UserRole.Teacher)
        {
            query = query.Where(s => s.Assignment.TeacherId == _currentUser.UserId);
        }
        // Admin: unrestricted.

        if (request.AssignmentId.HasValue)
        {
            query = query.Where(s => s.AssignmentId == request.AssignmentId.Value);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(s => s.StudentId == request.StudentId.Value);
        }

        return await query
            .OrderByDescending(s => s.SubmittedAt)
            .Select(s => new SubmissionDto(
                s.Id, s.AssignmentId, s.Assignment.Title, s.Assignment.MaxMarks, s.Assignment.Deadline,
                s.StudentId, s.Student.FullName, s.AnswerText, s.SubmittedAt, s.Status.ToString(),
                s.Marks, s.Feedback, s.GradedAt, s.GradedBy != null ? s.GradedBy.FullName : null))
            .ToListAsync(cancellationToken);
    }
}
