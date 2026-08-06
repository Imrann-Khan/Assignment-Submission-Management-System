using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Submissions.Grade;

public class GradeSubmissionCommandValidator : AbstractValidator<GradeSubmissionCommand>
{
    private readonly IApplicationDbContext _context;

    public GradeSubmissionCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Marks).GreaterThanOrEqualTo(0);

        RuleFor(x => x)
            .MustAsync(NotExceedMaxMarks)
            .WithMessage("Marks cannot exceed the assignment's maximum marks.")
            .OverridePropertyName("Marks");
    }

    private async Task<bool> NotExceedMaxMarks(GradeSubmissionCommand command, CancellationToken cancellationToken)
    {
        var maxMarks = await _context.Submissions
            .Where(s => s.Id == command.Id)
            .Select(s => (int?)s.Assignment.MaxMarks)
            .SingleOrDefaultAsync(cancellationToken);

        return maxMarks is null || command.Marks <= maxMarks.Value;
    }
}
