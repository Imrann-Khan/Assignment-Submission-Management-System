using Application.Common.Messaging;
using Application.Common.DTOs;

namespace Application.Subjects.Create;

public record CreateSubjectCommand(string Name, Guid ClassId) : IRequest<SubjectDto>;
