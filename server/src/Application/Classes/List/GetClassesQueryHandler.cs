using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Classes.List;

public class GetClassesQueryHandler : IRequestHandler<GetClassesQuery, List<ClassDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClassesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassDto>> Handle(GetClassesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Classes
            .Select(c => new ClassDto(
                c.Id,
                c.Name,
                c.Students.Count,
                c.Subjects.Select(s => new SubjectDto(s.Id, s.Name, s.ClassId)).ToList()))
            .ToListAsync(cancellationToken);
    }
}
