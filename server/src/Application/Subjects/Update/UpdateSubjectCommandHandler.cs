using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Domain.Entities;

namespace Application.Subjects.Update;

public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, SubjectDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubjectDto> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subjects.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Subject), request.Id);

        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);

        return new SubjectDto(entity.Id, entity.Name, entity.ClassId);
    }
}
