using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        entity.FullName = request.FullName;
        entity.Email = request.Email;

        if (entity.Role == UserRole.Student)
        {
            if (request.ClassId.HasValue)
            {
                var classExists = await _context.Classes.AnyAsync(c => c.Id == request.ClassId.Value, cancellationToken);
                if (!classExists)
                {
                    throw new NotFoundException(nameof(Class), request.ClassId.Value);
                }
            }

            entity.ClassId = request.ClassId;
        }

        await _context.SaveChangesAsync(cancellationToken);

        string? className = null;
        if (entity.ClassId.HasValue)
        {
            className = await _context.Classes
                .Where(c => c.Id == entity.ClassId.Value)
                .Select(c => c.Name)
                .SingleOrDefaultAsync(cancellationToken);
        }

        return new UserDto(entity.Id, entity.FullName, entity.Email, entity.Role.ToString(), entity.IsActive, entity.ClassId, className);
    }
}
