using Application.Common.Messaging;

namespace Application.Auth.Login;

public record LoginCommand(
    string Email, string Password
) : IRequest<LoginResult>;
