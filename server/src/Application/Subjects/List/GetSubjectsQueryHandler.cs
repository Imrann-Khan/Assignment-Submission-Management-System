using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Subjects.List;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, List<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Subjects.AsQueryable();

        if (request.ClassId.HasValue)
        {
            query = query.Where(s => s.ClassId == request.ClassId.Value);
        }

        return await query
            .Select(s => new SubjectDto(s.Id, s.Name, s.ClassId))
            .ToListAsync(cancellationToken);
    }
}
