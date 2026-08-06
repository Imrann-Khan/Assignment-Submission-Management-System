using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Entities;

namespace Application.Users.SetActiveStatus;

public class SetUserActiveStatusCommandHandler : IRequestHandler<SetUserActiveStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public SetUserActiveStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.Id);

        entity.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
