using Application.Common.DTOs;
using Application.Common.Messaging;
using Application.Common.Models;
using Domain.Enums;

namespace Application.Users.List;

public record GetUsersQuery(UserRole? Role, Guid? ClassId, int? PageNumber, int? PageSize) : IRequest<PagedResult<UserDto>>;
