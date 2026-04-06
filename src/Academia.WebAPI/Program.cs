using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Academia.Application;
using Academia.Infrastructure;
using Academia.Infrastructure.Auth;
using Academia.Infrastructure.Persistence;
using Academia.Infrastructure.Persistence.Seed;
using Academia.WebAPI.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            // Serialize enums as strings (e.g. "Published" instead of 2)
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    builder.Services.AddOpenApi();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── Health Checks ────────────────────────────────────────���─────────────────
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "postgres",
            tags: ["db", "ready"])
        .AddCheck("self", () => HealthCheckResult.Healthy());

    // ── Rate Limiting ──────────────────────────────────────────────────────────
    builder.Services.AddRateLimiter(options =>
    {
        // Strict: 5 requests/minute per IP — used on login endpoint
        options.AddFixedWindowLimiter("login", opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 5;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        });

        // General: 60 requests/minute per authenticated user (fallback: IP)
        options.AddFixedWindowLimiter("general", opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 60;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
        ?? throw new InvalidOperationException("JwtSettings section is missing.");

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("AdminOnly", p => p.RequireRole("Administrator"))
        .AddPolicy("AdminOrTeacher", p => p.RequireRole("Administrator", "Teacher"))
        .AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
            policy.WithOrigins(
                    "http://localhost:3000",
                    "https://academia.ibsg.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    var app = builder.Build();

    // ── X-Request-Id header middleware ─────────────────────────────────────────
    app.Use(async (context, next) =>
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];
        context.Response.Headers["X-Request-Id"] = requestId;
        await next();
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AcademiaDbContext>();
        await db.Database.MigrateAsync();
        Log.Information("Migrations applied successfully");
        await AcademiaSeeder.SeedAsync(
            db,
            scope.ServiceProvider.GetRequiredService<ILogger<AcademiaDbContext>>());
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseStaticFiles();
    app.UseCors("AllowFrontend");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapGet("/", () => Results.Ok(new
    {
        api = "IBSG Academia API",
        version = "1.0.0",
        status = "running",
        timestamp = DateTime.UtcNow
    }));

    Log.Information("IBSG Academia API starting...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
