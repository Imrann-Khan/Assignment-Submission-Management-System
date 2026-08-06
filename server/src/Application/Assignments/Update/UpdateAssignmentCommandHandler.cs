using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Assignments.Update;

public class UpdateAssignmentCommandHandler : IRequestHandler<UpdateAssignmentCommand, AssignmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAssignmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AssignmentDto> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Assignments.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Assignment), request.Id);

        if (entity.TeacherId != _currentUser.UserId)
        {
            throw new ForbiddenAccessException("You can only update assignments you created.");
        }

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Deadline = request.Deadline;
        entity.MaxMarks = request.MaxMarks;

        await _context.SaveChangesAsync(cancellationToken);

        return await _context.Assignments
            .Where(a => a.Id == entity.Id)
            .Select(a => new AssignmentDto(
                a.Id, a.Title, a.Description, a.Deadline, a.MaxMarks, a.Status.ToString(),
                a.ClassId, a.Class.Name, a.SubjectId, a.Subject.Name, a.TeacherId, a.Teacher.FullName, a.CreatedAt))
            .SingleAsync(cancellationToken);
    }
}
