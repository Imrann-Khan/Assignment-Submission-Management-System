using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Subjects.Create;

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, SubjectDto>
{
    private readonly IApplicationDbContext _context;

    public CreateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubjectDto> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var classExists = await _context.Classes.AnyAsync(c => c.Id == request.ClassId, cancellationToken);
        if (!classExists)
        {
            throw new NotFoundException(nameof(Class), request.ClassId);
        }

        var entity = new Subject { Name = request.Name, ClassId = request.ClassId };
        _context.Subjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SubjectDto(entity.Id, entity.Name, entity.ClassId);
    }
}
