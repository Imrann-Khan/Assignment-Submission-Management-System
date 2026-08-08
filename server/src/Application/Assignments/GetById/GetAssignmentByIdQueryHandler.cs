using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Assignments.GetById;

public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAssignmentByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AssignmentDto> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Assignments.Where(a => a.Id == request.Id);

        if (_currentUser.Role == UserRole.Teacher)
        {
            var teacherId = _currentUser.UserId!.Value;
            query = query.Where(a => a.TeacherId == teacherId);
        }
        else if (_currentUser.Role == UserRole.Student)
        {
            var currentUserId = _currentUser.UserId!.Value;
            var studentClassId = await _context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.ClassId)
                .SingleOrDefaultAsync(cancellationToken);

            if (studentClassId is null)
            {
                query = query.Where(a => false);
            }
            else
            {
                var classId = studentClassId.Value;
                query = query.Where(a => a.Status == AssignmentStatus.Published && a.ClassId == classId);
            }
        }

        var dto = await query
            .Select(a => new AssignmentDto(
                a.Id, a.Title, a.Description, a.Deadline, a.MaxMarks, a.Status.ToString(),
                a.ClassId, a.Class.Name, a.SubjectId, a.Subject.Name, a.TeacherId, a.Teacher.FullName, a.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);

        return dto ?? throw new NotFoundException(nameof(Assignment), request.Id);
    }
}
