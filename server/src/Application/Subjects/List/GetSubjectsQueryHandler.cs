using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Subjects.List;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, PagedResult<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SubjectDto>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Subjects.AsQueryable();

        if (request.ClassId.HasValue)
        {
            query = query.Where(s => s.ClassId == request.ClassId.Value);
        }

        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 20;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SubjectDto(s.Id, s.Name, s.ClassId))
            .ToListAsync(cancellationToken);

        return new PagedResult<SubjectDto>(items, totalCount, pageNumber, pageSize);
    }
}
