using Application.Common.DTOs;
using Application.Common.Messaging;
using Domain.Enums;

namespace Application.Users.Create;


public record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    Guid? ClassId
) : IRequest<UserDto>;