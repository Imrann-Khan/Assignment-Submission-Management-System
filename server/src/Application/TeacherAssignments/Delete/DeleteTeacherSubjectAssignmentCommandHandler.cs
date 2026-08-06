using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;

namespace Application.TeacherAssignments.Delete;

public record DeleteTeacherSubjectAssignmentCommandHandler : IRequestHandler<DeleteTeacherSubjectAssignmentCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteTeacherSubjectAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTeacherSubjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TeacherSubjectAssignments.FindAsync(new object[] {request.Id}, cancellationToken);

        if(entity == null)
            throw new NotFoundException(nameof(TeacherSubjectAssignment), request.Id);
        
        _context.TeacherSubjectAssignments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
