using Application.Common.Messaging;
using Application.Common.DTOs;

namespace Application.Classes.Create;

public record CreateClassCommand(string Name) : IRequest<ClassDto>;
