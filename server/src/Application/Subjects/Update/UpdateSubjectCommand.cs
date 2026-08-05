using Application.Common.Messaging;
using Application.Common.DTOs;

namespace Application.Subjects.Update;

public record UpdateSubjectCommand(Guid Id, string Name) : IRequest<SubjectDto>;
