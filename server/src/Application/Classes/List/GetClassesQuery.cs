using Application.Common.Messaging;
using Application.Common.DTOs;
using Application.Common.Models;

namespace Application.Classes.List;

public record GetClassesQuery(int? PageNumber, int? PageSize) : IRequest<PagedResult<ClassDto>>;
