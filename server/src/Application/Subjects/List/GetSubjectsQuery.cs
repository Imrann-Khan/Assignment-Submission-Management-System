using Application.Common.Messaging;
using Application.Common.DTOs;

namespace Application.Subjects.List;

public record GetSubjectsQuery(Guid? ClassId) : IRequest<List<SubjectDto>>;
