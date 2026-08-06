using Application.Common.Messaging;

namespace Application.Classes.Delete;

public record DeleteClassCommand(Guid Id) : IRequest<Unit>;
