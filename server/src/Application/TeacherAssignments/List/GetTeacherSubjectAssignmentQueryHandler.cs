using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Application.TeacherAssignments.List;

public class GetTeacherSubjectAssignmentQueryHandler : IRequestHandler<GetTeacherSubjectAssignmentQuery, List<TeacherSubjectAssignmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTeacherSubjectAssignmentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherSubjectAssignmentDto>> Handle(GetTeacherSubjectAssignmentQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TeacherSubjectAssignments.AsQueryable();

        if(request.TeacherId.HasValue)
        {
            query = query.Where(t => t.TeacherId == request.TeacherId.Value);
        }

        if(request.ClassId.HasValue)
        {
            query = query.Where(t => t.Subject.ClassId == request.ClassId.Value);
        }

        return await query
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
    }
}