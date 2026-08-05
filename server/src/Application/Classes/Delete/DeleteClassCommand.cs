using Application.Common.Messaging;

namespace Application.Classes.Delete;

public record DeleteClassCommand(int Id) : IRequest<Unit>;
