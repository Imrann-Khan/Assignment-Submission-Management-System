using FluentValidation;

namespace Application.Subjects.Create;

public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ClassId).NotEqual(Guid.Empty);
    }
}
