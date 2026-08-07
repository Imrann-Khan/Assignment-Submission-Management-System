using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.UnitTests.Common;

public class TestCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
}
