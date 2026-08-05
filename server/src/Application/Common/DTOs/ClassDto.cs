namespace Application.Common.DTOs;

public record ClassDto(
    Guid Id, 
    string Name, 
    int StudentCount, 
    List<SubjectDto> Subjects
);
