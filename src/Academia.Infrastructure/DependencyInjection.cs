using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using Academia.Infrastructure.Auth;
using Academia.Infrastructure.DataMigration;
using Academia.Infrastructure.Certificates;
using Academia.Infrastructure.Email;
using Academia.Infrastructure.Persistence;
using Academia.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Academia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AcademiaDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(AcademiaDbContext).Assembly.FullName)
            )
            .UseSnakeCaseNamingConvention()
        );

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AcademiaDbContext>());

        // Auth services
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Current user service (requires IHttpContextAccessor registered in WebAPI)
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // File storage
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Data migration service (used once during cutover)
        services.AddScoped<IMigrationService, DirectusMigrationService>();

        // Certificate generator
        services.AddSingleton<ICertificateGenerator, QuestPdfCertificateGenerator>();

        // Email service
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
