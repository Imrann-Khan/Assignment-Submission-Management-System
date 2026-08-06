using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Create;


public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = request.Role,
            ClassId = request.Role == UserRole.Student ? request.ClassId : null,
            IsActive = true
        };

        _context.Users.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        string? ClassName = null;
        if(entity.ClassId.HasValue)
        {
            ClassName = await _context.Classes
                                .Where(c => c.Id == entity.ClassId.Value)
                                .Select(c => c.Name)
                                .SingleOrDefaultAsync(cancellationToken);
        }

        return new UserDto(
            entity.Id,
            entity.FullName,
            entity.Email,
            entity.Role.ToString(),
            entity.IsActive,
            entity.ClassId,
            ClassName
        );
    }
}