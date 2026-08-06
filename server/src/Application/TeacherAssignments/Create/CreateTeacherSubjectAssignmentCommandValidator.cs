using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.TeacherAssignments.Create;


public class CreateTeacherSubjectAssignmentCommandValidator : AbstractValidator<CreateTeacherSubjectAssignmentCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTeacherSubjectAssignmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.TeacherId)
                .NotEqual(Guid.Empty)
                .MustAsync(BeAnExistingTeacher)
                .WithMessage("TeacherId must reference an existing Teacher");

        RuleFor(x => x.SubjectId)
                .NotEqual(Guid.Empty)
                .MustAsync(BeAnExistingSubject)
                .WithMessage("SubjectId must reference an existing Subject");

        RuleFor(x => x)
                .MustAsync(NotAlreadyBeAssigned)
                .WithMessage("This teacher already assigned to this subject")
                .OverridePropertyName("SubjectId");
    }

    private async Task<bool> BeAnExistingTeacher(Guid teacherId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == teacherId && u.Role == UserRole.Teacher, cancellationToken);
    }

    private async Task<bool> BeAnExistingSubject(Guid subjectId, CancellationToken cancellationToken)
    {
        return await _context.Subjects.AnyAsync(s => s.Id == subjectId, cancellationToken);
    }

    private async Task<bool> NotAlreadyBeAssigned(CreateTeacherSubjectAssignmentCommand command, CancellationToken cancellationToken)
    {
        return !await _context.TeacherSubjectAssignments.AnyAsync(
            t => t.TeacherId == command.TeacherId && t.SubjectId == command.SubjectId,
            cancellationToken
        );
    }
}