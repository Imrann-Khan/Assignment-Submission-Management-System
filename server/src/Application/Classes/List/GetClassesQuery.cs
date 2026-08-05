using Application.Common.Messaging;
using Application.Common.DTOs;

namespace Application.Classes.List;

public record GetClassesQuery : IRequest<List<ClassDto>>;
