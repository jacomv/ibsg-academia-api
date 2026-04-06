using System.Security.Cryptography;
using Academia.Application.Common.Email;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        // Always return success to prevent email enumeration attacks
        if (user is null || !user.IsActive) return;

        // Invalidate previous tokens
        var oldTokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed)
            .ToListAsync(cancellationToken);
        foreach (var old in oldTokens) old.MarkUsed();

        // Generate secure token
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        var token = new PasswordResetToken(user.Id, rawToken, DateTime.UtcNow.AddHours(1));
        await _context.PasswordResetTokens.AddAsync(token, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:3000";
        var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";

        try
        {
            await _emailService.SendAsync(new EmailMessage(
                To: user.Email,
                ToName: user.FullName,
                Subject: "Recuperar contraseña — IBSG Academia",
                HtmlBody: EmailTemplates.PasswordReset(user.FirstName, resetLink)
            ), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset email failed for {Email}", user.Email);
        }
    }
}
