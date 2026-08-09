using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.Models;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.TeacherAssignments.List;

public class GetTeacherSubjectAssignmentQueryHandler : IRequestHandler<GetTeacherSubjectAssignmentQuery, PagedResult<TeacherSubjectAssignmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTeacherSubjectAssignmentQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<TeacherSubjectAssignmentDto>> Handle(GetTeacherSubjectAssignmentQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TeacherSubjectAssignments.AsQueryable();

        if (_currentUser.Role == UserRole.Teacher)
        {
            var teacherId = _currentUser.UserId!.Value;
            query = query.Where(t => t.TeacherId == teacherId);
        }
        else if (request.TeacherId.HasValue)
        {
            query = query.Where(t => t.TeacherId == request.TeacherId.Value);
        }

        if (request.ClassId.HasValue)
        {
            query = query.Where(t => t.Subject.ClassId == request.ClassId.Value);
        }

        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 20;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.Teacher.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TeacherSubjectAssignmentDto(
                t.Id,
                t.TeacherId,
                t.Teacher.FullName,
                t.SubjectId,
                t.Subject.Name,
                t.Subject.ClassId,
                t.Subject.Class.Name
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<TeacherSubjectAssignmentDto>(items, totalCount, pageNumber, pageSize);
    }
}
