using Application.Common.Interfaces;
using Application.Common.Messaging;
using Application.Common.DTOs;
using Domain.Entities;

namespace Application.Classes.Create;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, ClassDto>
{
    private readonly IApplicationDbContext _context;

    public CreateClassCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassDto> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = new Class { Name = request.Name };
        _context.Classes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ClassDto(entity.Id, entity.Name, 0, new List<SubjectDto>());
    }
}
