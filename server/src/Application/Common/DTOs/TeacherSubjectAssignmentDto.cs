namespace Application.Common.DTOs;


public record TeacherSubjectAssignmentDto
(
    Guid Id,
    Guid TeacherId,
    string TeacherName,
    Guid SubjectId,
    string SubjectName,
    Guid ClassId,
    string ClassName
);