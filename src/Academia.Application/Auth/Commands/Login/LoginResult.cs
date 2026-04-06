using Academia.Domain.Enums;

namespace Academia.Application.Auth.Commands.Login;

public record LoginResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserInfo User
);

public record UserInfo(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role
);
