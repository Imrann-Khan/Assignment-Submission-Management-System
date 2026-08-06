using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Create;


public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IApplicationDbContext _context;
    
    public CreateUserCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(BeUniqueEmail).WithMessage("A user with this email already exists");
        
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

        RuleFor(x => x.Role).IsInEnum();

        RuleFor(x => x.ClassId)
                .NotNull().WithMessage("ClassId is required for students.")
                .MustAsync(BeAnExistingClass).WithMessage("ClassId must reference an existing Class.")
                .When(x => x.Role == UserRole.Student);

        RuleFor(x => x.ClassId)
                .Null().WithMessage("ClassId should not be set for non-student users.")
                .When(x => x.Role != UserRole.Student);
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        private async Task<bool> BeAnExistingClass(Guid? classId, CancellationToken cancellationToken)
        {
            return classId.HasValue && await _context.Classes.AnyAsync(c => c.Id == classId.Value, cancellationToken);
        }

}