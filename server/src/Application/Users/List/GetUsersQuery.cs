using Application.Common.DTOs;
using Application.Common.Messaging;
using Domain.Enums;

namespace Application.Users.List;

public record GetUsersQuery(UserRole? Role, Guid? ClassId) : IRequest<List<UserDto>>;
