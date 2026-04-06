using Academia.Application.Common.Email;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        ILogger<RegisterCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (emailExists)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["email"] = [$"The email '{request.Email}' is already registered."]
            });

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            _passwordHasher.Hash(request.Password),
            UserRole.Student);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Send welcome email (non-blocking)
        try
        {
            await _emailService.SendAsync(new EmailMessage(
                To: user.Email,
                ToName: user.FullName,
                Subject: "¡Bienvenido a IBSG Academia! 🙏",
                HtmlBody: EmailTemplates.Welcome(user.FirstName, user.Email)
            ), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Welcome email could not be sent to {Email}", user.Email);
        }

        return user.Id;
    }
}
