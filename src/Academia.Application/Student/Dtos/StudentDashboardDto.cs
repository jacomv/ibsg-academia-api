namespace Academia.Application.Student.Dtos;

public record StudentDashboardDto(
    int ActiveEnrollments,
    int CompletedLessons,
    int TotalLessons,
    decimal OverallProgress,
    decimal AverageScore,
    int UnreadNotifications,
    List<CourseInProgressDto> CoursesInProgress,
    List<RecentGradeDto> RecentGrades
);

public record CourseInProgressDto(
    Guid CourseId,
    string Title,
    string? Image,
    int CompletedLessons,
    int TotalLessons,
    decimal ProgressPercentage
);

public record RecentGradeDto(
    Guid GradeId,
    string ExamTitle,
    string CourseTitle,
    decimal TotalScore,
    bool IsPassed,
    DateTime CreatedAt
);
