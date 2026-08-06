using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Assignments.List;

public class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, List<AssignmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAssignmentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Assignments.AsQueryable();

        if (_currentUser.Role == UserRole.Teacher)
        {
            query = query.Where(a => a.TeacherId == _currentUser.UserId);
        }
        else if (_currentUser.Role == UserRole.Student)
        {
            var studentClassId = await _context.Users
                .Where(u => u.Id == _currentUser.UserId)
                .Select(u => u.ClassId)
                .SingleOrDefaultAsync(cancellationToken);

            query = query.Where(a => a.Status == AssignmentStatus.Published && a.ClassId == studentClassId);
        }
        // Admin: no forced restriction — sees everything, filtered only by the optional params below.

        if (request.ClassId.HasValue)
        {
            query = query.Where(a => a.ClassId == request.ClassId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(a => a.SubjectId == request.SubjectId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AssignmentDto(
                a.Id, a.Title, a.Description, a.Deadline, a.MaxMarks, a.Status.ToString(),
                a.ClassId, a.Class.Name, a.SubjectId, a.Subject.Name, a.TeacherId, a.Teacher.FullName, a.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
