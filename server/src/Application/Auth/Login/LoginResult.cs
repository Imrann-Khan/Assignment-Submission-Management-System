namespace Application.Auth.Login;

public record LoginResult(
    string Token,
    Guid UserId,
    string FullName,
    string Email,
    string Role
);
