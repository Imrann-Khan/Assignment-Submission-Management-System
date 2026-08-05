using Domain.Enums;

namespace Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId {get;}
    string? Email {get;}
    UserRole? Role {get;}
}