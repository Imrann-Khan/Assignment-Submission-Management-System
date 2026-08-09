using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Classes.List;

public class GetClassesQueryHandler : IRequestHandler<GetClassesQuery, PagedResult<ClassDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClassesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ClassDto>> Handle(GetClassesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 20;

        var totalCount = await _context.Classes.CountAsync(cancellationToken);

        var items = await _context.Classes
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClassDto(
                c.Id,
                c.Name,
                c.Students.Count,
                c.Subjects.Select(s => new SubjectDto(s.Id, s.Name, s.ClassId)).ToList()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ClassDto>(items, totalCount, pageNumber, pageSize);
    }
}
