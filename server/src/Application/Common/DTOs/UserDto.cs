namespace Application.Common.DTOs;

public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    Guid? ClassId,
    string? ClassName
);