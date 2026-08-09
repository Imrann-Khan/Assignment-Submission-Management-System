using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.List;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        if (request.Role.HasValue)
        {
            query = query.Where(u => u.Role == request.Role.Value);
        }

        if (request.ClassId.HasValue)
        {
            query = query.Where(u => u.ClassId == request.ClassId.Value);
        }

        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 20;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Email,
                u.Role.ToString(),
                u.IsActive,
                u.ClassId,
                u.Class == null ? null : u.Class.Name))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserDto>(items, totalCount, pageNumber, pageSize);
    }
}
