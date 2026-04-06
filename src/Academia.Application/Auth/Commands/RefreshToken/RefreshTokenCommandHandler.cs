using Academia.Application.Auth.Commands.Login;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainRefreshToken = Academia.Domain.Entities.RefreshToken;

namespace Academia.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.Token, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        // Rotate: revoke current, issue new
        storedToken.Revoke();

        var newRawToken = _jwtService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtService.RefreshTokenExpirationDays);

        await _context.RefreshTokens.AddAsync(
            new DomainRefreshToken(storedToken.UserId, newRawToken, expiresAt), cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var user = storedToken.User;
        return new LoginResult(
            AccessToken: _jwtService.GenerateAccessToken(user),
            RefreshToken: newRawToken,
            ExpiresIn: _jwtService.AccessTokenExpirationMinutes * 60,
            User: new UserInfo(user.Id, user.FirstName, user.LastName, user.Email, user.Role)
        );
    }
}
