namespace Application.Common.DTOs;

public record SubjectDto(
    Guid Id,
    string Name,
    Guid ClassId
);