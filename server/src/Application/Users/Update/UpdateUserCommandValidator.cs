using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x)
            .MustAsync(HaveUniqueEmail)
            .WithMessage("A user with this email already exists.")
            .OverridePropertyName("Email");
    }

    private async Task<bool> HaveUniqueEmail(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Email == command.Email && u.Id != command.Id, cancellationToken);
    }
}
