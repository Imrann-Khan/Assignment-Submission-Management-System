using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Assignments.Create;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateAssignmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId!.Value;

        var isAssignedToSubject = await _context.TeacherSubjectAssignments
            .AnyAsync(t => t.TeacherId == teacherId && t.SubjectId == request.SubjectId, cancellationToken);

        if (!isAssignedToSubject)
        {
            throw new ForbiddenAccessException("You are not assigned to teach this subject.");
        }

        var entity = new Assignment
        {
            Title = request.Title,
            Description = request.Description,
            ClassId = request.ClassId,
            SubjectId = request.SubjectId,
            TeacherId = teacherId,
            Deadline = request.Deadline,
            MaxMarks = request.MaxMarks,
            Status = AssignmentStatus.Draft
        };

        _context.Assignments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.Assignments
            .Where(a => a.Id == entity.Id)
            .Select(a => new AssignmentDto(
                a.Id, a.Title, a.Description, a.Deadline, a.MaxMarks, a.Status.ToString(),
                a.ClassId, a.Class.Name, a.SubjectId, a.Subject.Name, a.TeacherId, a.Teacher.FullName, a.CreatedAt))
            .SingleAsync(cancellationToken);
    }
}
