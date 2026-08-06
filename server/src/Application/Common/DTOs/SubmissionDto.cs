namespace Application.Common.DTOs;

public record SubmissionDto(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    int MaxMarks,
    DateTime Deadline,
    Guid StudentId,
    string StudentName,
    string AnswerText,
    DateTime SubmittedAt,
    string Status,
    int? Marks,
    string? Feedback,
    DateTime? GradedAt,
    string? GradedByName);
