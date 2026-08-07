using Application.Auth.Login;
using Application.Common.Interfaces;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenAndUserInfo()
    {
        using var context = TestDbContextFactory.Create();

        var user = new User
        {
            FullName = "Test Teacher",
            Email = "teacher@test.com",
            PasswordHash = "hashed-password",
            Role = UserRole.Teacher,
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync(CancellationToken.None);

        _passwordHasher.Setup(x => x.Verify("correct-password", "hashed-password")).Returns(true);
        _jwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("fake-jwt-token");

        var handler = new LoginCommandHandler(context, _passwordHasher.Object, _jwtTokenGenerator.Object);
        var command = new LoginCommand("teacher@test.com", "correct-password");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Token.Should().Be("fake-jwt-token");
        result.Email.Should().Be("teacher@test.com");
        result.Role.Should().Be("Teacher");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        using var context = TestDbContextFactory.Create();

        var user = new User
        {
            FullName = "Test Teacher",
            Email = "teacher@test.com",
            PasswordHash = "hashed-password",
            Role = UserRole.Teacher,
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync(CancellationToken.None);

        _passwordHasher.Setup(x => x.Verify("wrong-password", "hashed-password")).Returns(false);

        var handler = new LoginCommandHandler(context, _passwordHasher.Object, _jwtTokenGenerator.Object);
        var command = new LoginCommand("teacher@test.com", "wrong-password");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ThrowsUnauthorizedAccessException()
    {
        using var context = TestDbContextFactory.Create();

        var handler = new LoginCommandHandler(context, _passwordHasher.Object, _jwtTokenGenerator.Object);
        var command = new LoginCommand("nobody@test.com", "any-password");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithInactiveUser_ThrowsUnauthorizedAccessException()
    {
        using var context = TestDbContextFactory.Create();

        var user = new User
        {
            FullName = "Deactivated Teacher",
            Email = "teacher@test.com",
            PasswordHash = "hashed-password",
            Role = UserRole.Teacher,
            IsActive = false
        };
        context.Users.Add(user);
        await context.SaveChangesAsync(CancellationToken.None);

        _passwordHasher.Setup(x => x.Verify("correct-password", "hashed-password")).Returns(true);

        var handler = new LoginCommandHandler(context, _passwordHasher.Object, _jwtTokenGenerator.Object);
        var command = new LoginCommand("teacher@test.com", "correct-password");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
