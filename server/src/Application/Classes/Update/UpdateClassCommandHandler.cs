using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Classes.Update;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, ClassDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateClassCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassDto> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Classes.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Class), request.Id);

        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);

        var studentCount = await _context.Users.CountAsync(u => u.ClassId == entity.Id, cancellationToken);
        var subjects = await _context.Subjects
            .Where(s => s.ClassId == entity.Id)
            .Select(s => new SubjectDto(s.Id, s.Name, s.ClassId))
            .ToListAsync(cancellationToken);

        return new ClassDto(entity.Id, entity.Name, studentCount, subjects);
    }
}
