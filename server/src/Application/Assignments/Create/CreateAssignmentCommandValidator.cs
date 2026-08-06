using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Assignments.Create;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAssignmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
        RuleFor(x => x.MaxMarks).GreaterThan(0);
        RuleFor(x => x.ClassId).NotEqual(Guid.Empty);

        RuleFor(x => x.SubjectId)
            .NotEqual(Guid.Empty)
            .MustAsync(async (command, subjectId, cancellationToken) =>
                await _context.Subjects.AnyAsync(s => s.Id == subjectId && s.ClassId == command.ClassId, cancellationToken))
            .WithMessage("The selected subject does not belong to the selected class.");
    }
}
