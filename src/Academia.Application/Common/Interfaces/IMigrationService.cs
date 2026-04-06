namespace Academia.Application.Common.Interfaces;

public record MigrationResult(
    bool Success,
    string Message,
    MigrationStats Stats,
    List<string> Errors
);

public record MigrationStats(
    int Users,
    int Courses,
    int Chapters,
    int Lessons,
    int LearningPaths,
    int Exams,
    int Questions,
    int Enrollments,
    int UserProgress,
    int ExamAnswers,
    int Grades,
    int Notifications
);

public interface IMigrationService
{
    Task<MigrationResult> MigrateFromDirectusAsync(
        string directusConnectionString,
        CancellationToken ct = default);
}
