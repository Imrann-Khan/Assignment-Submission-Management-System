namespace Application.Common.DTOs;


public record NotificationDto(
    Guid Id,
    string Type,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    Guid? RelatedAssignmentId
);