using FluentValidation;

namespace Application.Submissions.Submit;

public class SubmitAssignmentCommandValidator : AbstractValidator<SubmitAssignmentCommand>
{
    public SubmitAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId).NotEqual(Guid.Empty);
        RuleFor(x => x.AnswerText).NotEmpty();
    }
}
