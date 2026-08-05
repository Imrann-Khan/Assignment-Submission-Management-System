using Application.Common.Messaging;
using Application.Common.DTOs;

namespace Application.Classes.Update;

public record UpdateClassCommand(Guid Id, string Name) : IRequest<ClassDto>;
