using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken);

        if (token is null || !token.IsValid)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["token"] = ["El enlace de recuperación es inválido o ha expirado."]
            });

        token.User.ChangePassword(_passwordHasher.Hash(request.NewPassword));
        token.MarkUsed();

        // Revoke all refresh tokens for security
        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == token.UserId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);
        foreach (var rt in refreshTokens) rt.Revoke();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
