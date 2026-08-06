using Application.Common.DTOs;
using Application.Common.Messaging;

namespace Application.Users.Update;

public record UpdateUserCommand(Guid Id, string FullName, string Email, Guid? ClassId) : IRequest<UserDto>;
