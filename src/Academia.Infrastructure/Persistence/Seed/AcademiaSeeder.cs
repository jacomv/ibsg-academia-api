using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Infrastructure.Persistence.Seed;

public static class AcademiaSeeder
{
    public static async Task SeedAsync(AcademiaDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding database...");

        var admin = new User(
            firstName: "Admin",
            lastName: "IBSG",
            email: "admin@ibsg.app",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            role: UserRole.Administrator
        );

        var teacher = new User(
            firstName: "Teacher",
            lastName: "Demo",
            email: "teacher@ibsg.app",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
            role: UserRole.Teacher
        );

        var student = new User(
            firstName: "Student",
            lastName: "Demo",
            email: "student@ibsg.app",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Student123!"),
            role: UserRole.Student
        );

        context.Users.AddRange(admin, teacher, student);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded 3 users: admin@ibsg.app, teacher@ibsg.app, student@ibsg.app");
    }
}
