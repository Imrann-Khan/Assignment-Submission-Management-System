using Application.Common.Messaging;
using Application.Common.DTOs;
using Application.Common.Models;

namespace Application.Subjects.List;

public record GetSubjectsQuery(Guid? ClassId, int? PageNumber, int? PageSize) : IRequest<PagedResult<SubjectDto>>;
