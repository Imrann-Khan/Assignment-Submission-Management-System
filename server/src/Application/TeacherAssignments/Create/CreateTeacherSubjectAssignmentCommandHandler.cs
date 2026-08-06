using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.TeacherAssignments.Create;
public class CreateTeacherSubjectAssignmentCommandHandler : IRequestHandler<CreateTeacherSubjectAssignmentCommand, TeacherSubjectAssignmentDto>
{
    private readonly IApplicationDbContext _context;

    public CreateTeacherSubjectAssignmentCommandHandler(IApplicationDbContext context, CancellationToken cancellationToken)
    {
        _context = context;
    }

    public async Task<TeacherSubjectAssignmentDto> Handle(CreateTeacherSubjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        var entity = new TeacherSubjectAssignment
        {
            TeacherId = request.TeacherId,
            SubjectId = request.SubjectId
        };

        _context.TeacherSubjectAssignments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.TeacherSubjectAssignments
                    .Where(t => t.Id == entity.Id)
                    .Select(t => new TeacherSubjectAssignmentDto(
                        t.Id,
                        t.TeacherId,
                        t.Teacher.FullName,
                        t.SubjectId,
                        t.Subject.Name,
                        t.Subject.ClassId,
                        t.Subject.Class.Name))
                    .SingleAsync(cancellationToken);
    }
}