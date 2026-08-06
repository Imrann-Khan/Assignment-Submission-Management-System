using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.List;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
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

        return await query
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Email,
                u.Role.ToString(),
                u.IsActive,
                u.ClassId,
                u.Class == null ? null : u.Class.Name))
            .ToListAsync(cancellationToken);
    }
}
